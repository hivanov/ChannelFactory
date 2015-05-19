using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChannelFactory.Channels;

namespace ChannelFactory.Factory
{
    public class ChannelCreationFactory<TInterface> : IChannelCreationFactory<ClientChannel<TInterface>, TInterface>
        where TInterface : class
    {
        #region Fields

        private readonly Uri m_AddressUri;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelCreationFactory{TInterface}"/> class.
        /// </summary>
        /// <param name="addressUri">The address URI.</param>
        public ChannelCreationFactory(Uri addressUri)
        {
            m_AddressUri = addressUri;
        }

        #endregion

        #region IChannelCreationFactory<ClientChannel<TInterface>,TInterface> Members

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public ClientChannel<TInterface> Create()
        {
            var channel = new ClientChannel<TInterface>(m_AddressUri);
            return channel;
        }

        /// <summary>
        /// Releases the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public void Release(ClientChannel<TInterface> channel)
        {
            if (channel is IDisposable)
            {
                ((IDisposable)channel).Dispose();
            }
        }

        #endregion
    }
}
