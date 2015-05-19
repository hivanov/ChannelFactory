using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using ChannelFactory.Channels;
using ChannelFactory.Contracts;
using ChannelFactory.Interceptors;
using ChannelFactory.Utils;

namespace ChannelFactory.Pipeline
{
    /// <summary>
    /// Interceptor pipeline factory
    /// </summary>
    class InterceptorPipelineFactory: IInterceptorPipelineFactory
    {
        #region Fields
        private readonly IKernel m_Kernel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorPipelineFactory"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <exception cref="System.ArgumentNullException">kernel</exception>
        public InterceptorPipelineFactory(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            m_Kernel = kernel;
        }

        #endregion

        #region IInterceptorPipelineFactory Members

        /// <summary>
        /// Creates the interceptors.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="createChannelBeforeEveryOperation">if set to <c>true</c> [create channel before every operation].</param>
        /// <param name="channelCreationFactory">The channel creation factory.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IInterceptor[] CreateInterceptors<TChannel, TInterface>(
            bool createChannelBeforeEveryOperation, 
            IChannelCreationFactory<TChannel, TInterface> channelCreationFactory)
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            if (channelCreationFactory == null)
            {
                channelCreationFactory = ChannelUtilities.ResolveChannelCreationFactory<TChannel, TInterface>(m_Kernel);
            }

            if (channelCreationFactory == null)
            {
                throw new ComponentResolutionException(
                    string.Format(
                        "Component IChannelCreationFactory<{0}, {1}> not registered",
                        typeof(TChannel).Name, typeof(TInterface).Name));
            }
            var context = new PipelineContext<TChannel, TInterface>();
            var interceptors = new List<IInterceptor>();
            if (createChannelBeforeEveryOperation)
            {
                interceptors.Add(new ChannelCreationOnEveryInvokeOperationInterceptor<TChannel, TInterface>(
                    context, channelCreationFactory));
                interceptors.Add(new EmptyIDisposableInterceptor());
            }
            else
            {
                var channelInterceptor = new ChannelCreationOnInitializationInterceptor<TChannel, TInterface>(context, channelCreationFactory);
                interceptors.Add(channelInterceptor);
                interceptors.Add(new DisposeSpecificInstancesInterceptor(channelInterceptor));
            }
            interceptors.Add(new ScopingInterceptor<TChannel, TInterface>(context));
            interceptors.AddRange(ChannelInterfaceImplementationFactory<TChannel, TInterface>.GetInterceptors(context));

            return interceptors.ToArray();
        }

        #endregion
    }
}
