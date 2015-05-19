using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Pipeline;

namespace ChannelFactory.Interceptors
{
    /// <summary>
    /// A network context interface implementation factory
    /// </summary>
    /// <typeparam name="TChannel">The type of the context.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    internal static class ChannelInterfaceImplementationFactory<TChannel, TInterface>
        where TChannel : ClientBase<TInterface>
        where TInterface : class
    {
        #region Fields

        private static readonly Type m_ReflectedType;
        private static readonly MethodInfo[] m_InterfaceMethods;
        private static readonly List<Func<PipelineContext<TChannel, TInterface>, IInterceptor>> m_CreationFuncs;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="ChannelInterfaceImplementationFactory{TChannel, TInterface}"/> class.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        static ChannelInterfaceImplementationFactory()
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface ||
                interfaceType.GetCustomAttribute<ServiceContractAttribute>() == null)
            {
                throw new NotSupportedException(
                    string.Format("Type {0} is not interface or is not attributed with ServiceContractAttribute", interfaceType.Name));
            }

            m_CreationFuncs = new List<Func<PipelineContext<TChannel, TInterface>, IInterceptor>>();
            m_InterfaceMethods = typeof(TInterface)
                .GetMembers()
                .OfType<MethodInfo>()
                .Where(m => m.GetCustomAttribute<OperationContractAttribute>() != null)
                .ToArray();

            var typeBuilder = CreateTypeAndMethodImplementations();

            m_ReflectedType = typeBuilder.CreateType();

            foreach (var interfaceMethod in m_InterfaceMethods)
            {
                var outParameters = GetOutParametersPositions(interfaceMethod);
                var reflectedMethod = m_ReflectedType.GetMethod(interfaceMethod.Name, BindingFlags.Public | BindingFlags.Static);

                if (outParameters.Count > 0)
                {
                    m_CreationFuncs.Add(
                        context =>
                            new NetworkInvokeAndCopyResultsBackInterceptor<TChannel, TInterface>(
                                context,
                                reflectedMethod,
                                outParameters.ToArray()));
                }
                else
                {
                    m_CreationFuncs.Add(
                        context =>
                            new NetworkInvokeInterceptor<TChannel, TInterface>(context, reflectedMethod));
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the out parameters positions.
        /// </summary>
        /// <param name="interfaceMethod">The interface method.</param>
        /// <returns></returns>
        private static List<int> GetOutParametersPositions(MethodInfo interfaceMethod)
        {
            var outParameters = new List<int>();
            var parameters = interfaceMethod.GetParameters();
            for (var i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].IsOut)
                {
                    outParameters.Add(i);
                }
            }
            return outParameters;
        }

        /// <summary>
        /// Creates the type and method implementations.
        /// </summary>
        /// <returns></returns>
        private static TypeBuilder CreateTypeAndMethodImplementations()
        {
            var assemblyName = GetAssemblyName();
            var physicalAssemblyName = GetPhysicalAssemblyName(assemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(typeof(TChannel).Name, physicalAssemblyName, true);
            var typeBuilder = moduleBuilder.DefineType(GetTypeImplementationName(), TypeAttributes.Public | TypeAttributes.Sealed);

            foreach (var interfaceMethod in m_InterfaceMethods)
            {
                var expressionTree = CreateInvokeOperation(interfaceMethod);
                var methodBuilder = typeBuilder.DefineMethod(
                    interfaceMethod.Name,
                    MethodAttributes.Static | MethodAttributes.Public,
                    CallingConventions.Standard,
                    interfaceMethod.ReturnType,
                    interfaceMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray());
                expressionTree.CompileToMethod(methodBuilder);
            }
            return typeBuilder;
        }
        
        /// <summary>
        /// Creates a lambda expression that takes the following parameters:
        /// - context (of type TChannel)
        /// - method arguments
        /// and returns the method return type
        /// </summary>
        /// interfaceMethod.ReturnType interfaceMethod.Name(context, [method_argument]*)
        /// {
        ///   return context.Channel.interfaceMethodName([method_argument]*);
        /// }
        /// <typeparam name="TChannel">The type of the context.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="interfaceMethod">The interface method.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">
        /// </exception>
        /// <exception cref="System.ArgumentNullException">interfaceMethod</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private static LambdaExpression CreateInvokeOperation(MethodInfo interfaceMethod)
        {
            var interfaceType = typeof(TInterface);
            if (interfaceMethod == null) throw new ArgumentNullException("interfaceMethod");
            if (interfaceMethod.DeclaringType != interfaceType)
            {
                throw new ArgumentException(
                    string.Format("Interface {0} does not declare method {1}", interfaceType.Name, interfaceMethod.Name));
            }
            if (interfaceMethod.GetCustomAttribute<OperationContractAttribute>() == null)
            {
                throw new NotSupportedException(
                    string.Format("Method {0} is not attributed with OperationContractAttribute", interfaceMethod.Name));
            }

            var interfaceMethodArguments = interfaceMethod
                .GetParameters()
                .Where(parameter => !parameter.IsRetval)
                .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
                .ToList();
            var channelParameter = Expression.Parameter(typeof(TInterface));
            var methodCall = Expression.Call(channelParameter, interfaceMethod, interfaceMethodArguments);

            var function = Expression.Lambda(methodCall, channelParameter.Yield().Concat(interfaceMethodArguments));
            return function;
        }

        /// <summary>
        /// Gets the name of the type implementation.
        /// </summary>
        /// <returns></returns>
        private static string GetTypeImplementationName()
        {
            return typeof(TInterface).Name + "Implementation";
        }

        /// <summary>
        /// Gets the name of the physical assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        private static string GetPhysicalAssemblyName(AssemblyName assemblyName)
        {
            var physicalAssemblyName = assemblyName.Name + ".dll";
            return physicalAssemblyName;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <returns></returns>
        private static AssemblyName GetAssemblyName()
        {
            var assemblyName = new AssemblyName(
                GetAssemblyNameString());
            return assemblyName;
        }

        /// <summary>
        /// Gets the assembly name string.
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyNameString()
        {
            return string.Format("ChannelFactory.CachedInvocationMethods.{0}.{1}",
                typeof(TChannel).Name,
                typeof(TInterface).Name);
        }

        /// <summary>
        /// Gets the interceptors.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public static IEnumerable<IInterceptor> GetInterceptors(PipelineContext<TChannel, TInterface> context)
        {
            if (context == null) throw new ArgumentNullException("context");

            foreach (var func in m_CreationFuncs)
            {
                yield return func(context);
            }
        }
    }
}
