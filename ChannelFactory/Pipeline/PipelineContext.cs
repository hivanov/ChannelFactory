using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChannelFactory.Channels;

namespace ChannelFactory.Pipeline
{
    /// <summary>
    /// The pipeline context
    /// </summary>
    /// <typeparam name="TChannel">The type of the context.</typeparam>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    internal class PipelineContext<TChannel, TInterface>
        where TChannel: ClientBase<TInterface>
        where TInterface: class
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public TChannel Channel { get; set; }
    }
}
