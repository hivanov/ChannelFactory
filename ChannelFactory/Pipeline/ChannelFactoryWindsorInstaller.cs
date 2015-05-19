using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ChannelFactory.Contracts;
using ChannelFactory.Logic;
using F = ChannelFactory.Factory;

namespace ChannelFactory.Pipeline
{
    /// <summary>
    /// Channel factory Windsor installer
    /// </summary>
    public class ChannelFactoryWindsorInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<InterceptorSelector>().LifeStyle.Singleton,
                Component.For<ProxyGenerator>().LifeStyle.Singleton,
                Component.For<IInterceptorPipelineFactory>().ImplementedBy<InterceptorPipelineFactory>().LifeStyle.Singleton,
                Component.For<IChannelFactory>().ImplementedBy<F.ChannelFactory>().LifeStyle.Singleton
            );
        }

        #endregion
    }
}
