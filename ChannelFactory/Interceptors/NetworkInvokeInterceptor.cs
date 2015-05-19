using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Contracts;
using ChannelFactory.Pipeline;
using ChannelFactory.Utils;

namespace ChannelFactory.Interceptors
{
    /// <summary>
    /// Invokes a network operation
    /// </summary>
    /// <typeparam name="TChannel">The type of the context.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    internal class NetworkInvokeInterceptor<TChannel, TInterface> : ISpecificMethodInterceptor
        where TChannel : ClientBase<TInterface>
        where TInterface : class
    {
        #region Fields

        private readonly PipelineContext<TChannel, TInterface> m_Context;
        private readonly MethodInfo m_MethodToInvoke;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkInvokeInterceptor{TChannel, TInterface}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="methodToInvoke">The method to invoke.</param>
        /// <param name="argumentIndexesForTransfer">The argument indexes for transfer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// methodToInvoke
        /// or
        /// argumentIndexesForTransfer
        /// </exception>
        public NetworkInvokeInterceptor(
            PipelineContext<TChannel, TInterface> context, 
            MethodInfo methodToInvoke)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (methodToInvoke == null) throw new ArgumentNullException("methodToInvoke");

            m_Context = context;
            m_MethodToInvoke = methodToInvoke;
        }

        #endregion

        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = m_MethodToInvoke.Invoke(
                null,
                ChannelUtilities.GetDelegateInvokeArguments<TChannel, TInterface>(
                    m_Context.Channel, 
                    invocation.Arguments));
        }

        #endregion

        #region ISpecificMethodInterceptor Members

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName
        {
            get 
            {
                return m_MethodToInvoke.Name;
            }
        }

        #endregion
    }
}
