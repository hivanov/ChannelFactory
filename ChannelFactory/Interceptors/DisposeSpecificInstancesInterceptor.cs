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
    /// An IDisposable enforcer interceptor
    /// </summary>
    class DisposeSpecificInstancesInterceptor : ISpecificMethodInterceptor
    {
        #region Fields

        private readonly IDisposable m_Subject;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeSpecificInstancesInterceptor"/> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        public DisposeSpecificInstancesInterceptor(IDisposable subject)
        {
            if (subject == null) throw new ArgumentNullException("subject");

            m_Subject = subject;
        }

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
            m_Subject.Dispose();
        }

        #endregion
    }
}
