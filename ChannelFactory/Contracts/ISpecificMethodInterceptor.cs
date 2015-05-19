using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace ChannelFactory.Contracts
{
    /// <summary>
    /// An interface for a specific method
    /// </summary>
    internal interface ISpecificMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        string MethodName { get; }
    }
}
