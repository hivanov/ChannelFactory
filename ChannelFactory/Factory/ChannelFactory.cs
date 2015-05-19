using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Channels;
using ChannelFactory.Contracts;
using ChannelFactory.Logic;

namespace ChannelFactory.Factory
{
    /// <summary>
    /// The channel factory itself
    /// </summary>
    class ChannelFactory : IChannelFactory
    {

        #region Fields

        private readonly ProxyGenerator m_ProxyGenerator;
        private readonly IInterceptorPipelineFactory m_InterceptorPipelineFactory;
        private readonly InterceptorSelector m_InterceptorSelector;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFactory" /> class.
        /// </summary>
        /// <param name="proxyGenerator">The proxy generator.</param>
        /// <param name="interceptorPipelineFactory">The interceptor pipeline factory.</param>
        /// <param name="interceptorSelector">The interceptor selector.</param>
        /// <exception cref="System.ArgumentNullException">
        /// proxyGenerator
        /// or
        /// interceptorPipelineFactory
        /// or
        /// interceptorSelector
        /// </exception>
        public ChannelFactory(
            ProxyGenerator proxyGenerator, 
            IInterceptorPipelineFactory interceptorPipelineFactory,
            InterceptorSelector interceptorSelector
        )
        {
            if (proxyGenerator == null) throw new ArgumentNullException("proxyGenerator");
            if (interceptorPipelineFactory == null) throw new ArgumentNullException("interceptorPipelineFactory");
            if (interceptorSelector == null) throw new ArgumentNullException("interceptorSelector");

            m_ProxyGenerator = proxyGenerator;
            m_InterceptorPipelineFactory = interceptorPipelineFactory;
            m_InterceptorSelector = interceptorSelector;
        }

        #region IChannelFactory Members

        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="createChannelOnEveryInvokeOperation">if set to <c>true</c> creates a new channel on every invoke operation.</param>
        /// <param name="channelCreationFactory">The channel creation factory.</param>
        /// <returns></returns>
        public TInterface Create<TChannel, TInterface>(
            bool createChannelOnEveryInvokeOperation = true,
            IChannelCreationFactory<TChannel, TInterface> channelCreationFactory = null
        )
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            var proxyCreationOptions = new ProxyGenerationOptions
            {
                Selector = m_InterceptorSelector,
            };

            var interceptors = m_InterceptorPipelineFactory.CreateInterceptors(createChannelOnEveryInvokeOperation, channelCreationFactory);

            var result = m_ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(
                proxyCreationOptions, 
                interceptors);

            return result;
        }

        /// <summary>
        /// Releases the specified channel.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="channel">The channel.</param>
        public void Release<TInterface>(TInterface channel)
            where TInterface : class
        {
            if (channel is IDisposable)
            {
                ((IDisposable)channel).Dispose();
            }
        }

        #endregion
    }
}
