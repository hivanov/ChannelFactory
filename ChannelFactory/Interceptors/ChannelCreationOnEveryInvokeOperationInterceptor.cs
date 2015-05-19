using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Channels;
using ChannelFactory.Pipeline;
using ChannelFactory.Utils;

namespace ChannelFactory.Interceptors
{
    /// <summary>
    /// Creates a custom channel on every invoke
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    internal class ChannelCreationOnEveryInvokeOperationInterceptor<TChannel, TInterface> : IInterceptor
        where TChannel: ClientBase<TInterface>
        where TInterface: class
    {
        #region Fields

        private readonly PipelineContext<TChannel, TInterface> m_Context;
        private readonly IChannelCreationFactory<TChannel, TInterface> m_ChannelFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelCreationOnEveryInvokeOperationInterceptor{TChannel, TInterface}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        public ChannelCreationOnEveryInvokeOperationInterceptor(
            PipelineContext<TChannel, TInterface> context,
            IChannelCreationFactory<TChannel, TInterface> channelFactory
        )
        {
            if (context == null) throw new ArgumentNullException("context");
            if (channelFactory == null) throw new ArgumentNullException("channelFactory");

            m_Context = context;
            m_ChannelFactory = channelFactory;
        }

        #endregion

        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            m_Context.Channel = m_ChannelFactory.Create();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                if (m_Context.Channel != null)
                {
                    m_ChannelFactory.Release(m_Context.Channel);
                }
            }
        }

        #endregion
    }
}
