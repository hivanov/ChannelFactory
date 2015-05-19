using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Pipeline;

namespace ChannelFactory.Interceptors
{
    /// <summary>
    /// A scoping interceptor, assigning a scope to each operation before invocation
    /// </summary>
    /// <typeparam name="TChannel">The type of the context.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    internal class ScopingInterceptor<TChannel, TInterface> : IInterceptor
        where TChannel : ClientBase<TInterface>
        where TInterface : class
    {
        #region Fields

        private readonly PipelineContext<TChannel, TInterface> m_Context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopingInterceptor{TChannel, TInterface}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public ScopingInterceptor(PipelineContext<TChannel, TInterface> context)
        {
            if (context == null) throw new ArgumentNullException("context");

            m_Context = context;
        }

        #endregion

        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            using (var scope = new OperationContextScope(m_Context.Channel.InnerChannel))
            {
                invocation.Proceed();
            }
        }

        #endregion
    }
}
