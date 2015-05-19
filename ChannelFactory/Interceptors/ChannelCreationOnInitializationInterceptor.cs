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
    /// Creates the channel on initialization
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    class ChannelCreationOnInitializationInterceptor<TChannel, TInterface> : IInterceptor, IDisposable
        where TChannel: ClientBase<TInterface>
        where TInterface: class
    {
        #region Fields

        private readonly PipelineContext<TChannel, TInterface> m_Context;
        private readonly IChannelCreationFactory<TChannel, TInterface> m_ChannelCreationFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelCreationOnInitializationInterceptor{TChannel, TInterface}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="channelCreationFactory">The channel creation factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// channelCreationFactory
        /// </exception>
        public ChannelCreationOnInitializationInterceptor(
            PipelineContext<TChannel, TInterface> context,
            IChannelCreationFactory<TChannel, TInterface> channelCreationFactory
        )
        {
            if (context == null) throw new ArgumentNullException("context");
            if (channelCreationFactory == null) throw new ArgumentNullException("channelCreationFactory");

            m_Context = context;
            m_ChannelCreationFactory = channelCreationFactory;

            m_Context.Channel = m_ChannelCreationFactory.Create();
        }

        #endregion

        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_Context.Channel != null)
            {
                m_ChannelCreationFactory.Release(m_Context.Channel);
            }
        }

        #endregion
    }
}
