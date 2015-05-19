using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ChannelFactory.Channels;
using ChannelFactory.Contracts;
using ChannelFactory.Factory;
using ChannelFactory.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        #region Fields

        private ServiceHost m_Host;
        private Uri m_ServiceUri;
        private WindsorContainer m_Container;
        private ChannelCreationFactory<ITestService> m_ChannelCreationFactory;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            var baseUri = new Uri("net.tcp://localhost:30145/");
            m_ServiceUri = new Uri(baseUri, "service");
            var binding = new NetTcpBinding();

            m_Host = new ServiceHost(typeof(TestService), m_ServiceUri);

            m_Host.AddServiceEndpoint(typeof(ITestService), binding, string.Empty);

            m_Host.Open();

            m_Container = new WindsorContainer();
            m_Container.Install(new ChannelFactoryWindsorInstaller());

            m_ChannelCreationFactory = new ChannelCreationFactory<ITestService>(m_ServiceUri);
        }

        #endregion

        #region Finalize

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        [TestCleanup]
        public void Shutdown()
        {
            m_Host.Close();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the ITestService channel.
        /// </summary>
        /// <returns></returns>
        private ITestService GetITestServiceChannel()
        {
            var factory = m_Container.Resolve<IChannelFactory>();

            var channel = factory.Create(channelCreationFactory: m_ChannelCreationFactory);
            return channel;
        }
        
        #endregion

        /// <summary>
        /// Tests the single fire.
        /// </summary>
        [TestMethod]
        public void TestSingleFire()
        {
            var channel = GetITestServiceChannel();

            var result = channel.CopyMe(1);

            Assert.AreEqual(1, result);
        }


        /// <summary>
        /// Invokes me multiple times.
        /// </summary>
        [TestMethod]
        public void InvokeMeMultipleTimes()
        {
            const int TIMES = 1000;

            var channel = GetITestServiceChannel();

            for (var i = 0; i < TIMES; ++i)
            {
                var result = channel.CopyMe(1);

                Assert.AreEqual(1, result);
            }
        }
    }
}
