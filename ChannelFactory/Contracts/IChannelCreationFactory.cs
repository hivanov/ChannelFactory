using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChannelFactory.Channels
{
    /// <summary>
    /// An interface for basic channel creation
    /// </summary>
    /// <typeparam name="TChannel">The type of the channel.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    public interface IChannelCreationFactory<TChannel, TInterface>
        where TChannel: ClientBase<TInterface>
        where TInterface: class
    {
        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <returns></returns>
        TChannel Create();

        /// <summary>
        /// Releases the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        void Release(TChannel channel);
    }
}
