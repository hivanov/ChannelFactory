using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace ChannelFactory.Logic
{
    /// <summary>
    /// Proxy generation hook
    /// </summary>
    class ProxyGenerationHook : IProxyGenerationHook
    {
        #region IProxyGenerationHook Members

        /// <summary>
        /// Invoked by the generation process to notify that the whole process has completed.
        /// </summary>
        public void MethodsInspected()
        {
            // nothing...
        }

        /// <summary>
        /// Invoked by the generation process to notify that a member was not marked as virtual.
        /// </summary>
        /// <param name="type">The type which declares the non-virtual member.</param>
        /// <param name="memberInfo">The non-virtual member.</param>
        /// <remarks>
        /// This method gives an opportunity to inspect any non-proxyable member of a type that has
        /// been requested to be proxied, and if appropriate - throw an exception to notify the caller.
        /// </remarks>
        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
            // again, nothing
        }

        /// <summary>
        /// Invoked by the generation process to determine if the specified method should be proxied.
        /// </summary>
        /// <param name="type">The type which declares the given method.</param>
        /// <param name="methodInfo">The method to inspect.</param>
        /// <returns>
        /// True if the given method should be proxied; false otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// methodInfo
        /// </exception>
        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            if (!type.IsInterface || type.GetCustomAttribute<ServiceContractAttribute>() == null)
            {
                return false;
            }
            return methodInfo.GetCustomAttribute<OperationContractAttribute>() != null;
        }

        #endregion
    }
}
