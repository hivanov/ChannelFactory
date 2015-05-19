using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ChannelFactory.Channels
{
    /// <summary>
    /// A simple (dummy) client channel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientChannel<T> : ClientBase<T>
        where T : class
    {
        #region Fields

        private readonly Uri m_AddressUri;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel{T}" /> class.
        /// </summary>
        /// <param name="addressUri">The address URI.</param>
        /// <exception cref="System.ArgumentNullException">addressUri</exception>
        public ClientChannel(Uri addressUri) :
            base()
        {
            if (addressUri == null) throw new ArgumentNullException("addressUri");
            m_AddressUri = addressUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel{T}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public ClientChannel(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel{T}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ClientChannel(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel{T}"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ClientChannel(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientChannel{T}"/> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ClientChannel(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {

        }

        #endregion

        #region Override

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <returns></returns>
        protected override T CreateChannel()
        {
            if (Endpoint.Address == null)
            {
                Endpoint.Address = new EndpointAddress(m_AddressUri);
            }
            return base.CreateChannel();
        }

        #endregion
    }
}
