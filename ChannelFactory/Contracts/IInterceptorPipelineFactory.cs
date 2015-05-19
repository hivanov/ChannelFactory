using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ChannelFactory.Channels;

namespace ChannelFactory.Contracts
{
    /// <summary>
    /// Interceptor pipeline factory interface
    /// </summary>
    interface IInterceptorPipelineFactory
    {
        /// <summary>
        /// Creates the interceptors.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="createChannelBeforeEveryOperation">if set to <c>true</c> [create channel before every operation].</param>
        /// <param name="channelCreationFactory">The channel creation factory.</param>
        /// <returns></returns>
        IInterceptor[] CreateInterceptors<TChannel, TInterface>(
            bool createChannelBeforeEveryOperation,
            IChannelCreationFactory<TChannel, TInterface> channelCreationFactory)
            where TChannel : ClientBase<TInterface>
            where TInterface : class;
    }
}
