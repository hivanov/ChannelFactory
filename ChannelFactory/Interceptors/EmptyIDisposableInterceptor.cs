using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Contracts;

namespace ChannelFactory.Interceptors
{
    /// <summary>
    /// An IDisposable implementation that does nothing
    /// </summary>
    class EmptyIDisposableInterceptor : ISpecificMethodInterceptor
    {
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
                return @"Dispose"; 
            }
        }

        #endregion

        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            
        }

        #endregion
    }
}
