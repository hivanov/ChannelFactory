using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChannelFactory.Channels;

namespace ChannelFactory.Contracts
{
    /// <summary>
    /// Channel factory interface
    /// </summary>
    public interface IChannelFactory
    {
        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="createChannelOnEveryInvokeOperation">if set to <c>true</c> creates a new channel on every invoke operation.</param>
        /// <param name="channelCreationFactory">The channel creation factory.</param>
        /// <returns></returns>
        TInterface Create<TChannel, TInterface>(
            bool createChannelOnEveryInvokeOperation = true, 
            IChannelCreationFactory<TChannel, TInterface> channelCreationFactory = null
        )
            where TChannel : ClientBase<TInterface>
            where TInterface : class;

        /// <summary>
        /// Releases the specified channel.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="channel">The channel.</param>
        void Release<TInterface>(TInterface channel)
            where TInterface : class;
    }
}
