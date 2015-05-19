using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using ChannelFactory.Channels;

namespace ChannelFactory.Utils
{
    internal class ChannelUtilities
    {
        private const string PATH_CHECK_REGEX = @"^(.*/)?(?:$|(.+?)(?:(\.[^.]*$)|$))";

        /// <summary>
        /// Gets the End Point Address to be used to communicate with the service
        /// </summary>
        /// <returns>An implementation of the target End Point Address</returns>
        /// <exception cref="ApplicationException"></exception>
        public static EndpointAddress GenerateEndPointAddress(ServiceEndpoint Endpoint, string serverBaseAddress, string serviceName)
        {
            EndpointAddress endPointAddress = null;

            // Verifies if the EndPoint exists, normally this means that the address tag exists in the config file.
            // In this case the address in the config file will be used unchanged.
            // If the address does not exist it will be generated based on the data passed through the constructor
            if (Endpoint.Address == null)
            {
                if (string.IsNullOrEmpty(serverBaseAddress))
                {
                    throw new ArgumentException("Argument serverBaseAddress cannot be null or empty");
                }

                if (string.IsNullOrEmpty(serviceName))
                {
                    throw new ArgumentException("Argument serviceName cannot be null or empty");
                }

                string bindingScheme = Endpoint.Binding.Scheme;
                Uri serverBaseAddressUri = null;

                try
                {
                    serverBaseAddressUri = new Uri(serverBaseAddress);
                }
                catch (UriFormatException)
                {
                    serverBaseAddressUri = new Uri(Path.Combine("dumb://", serverBaseAddress));
                }

                // Verifies if the server base address string was passed with or without the scheme prefix that identifies the binging (eg.: http) and adds it when is not present
                // Throws an exception when the address is not well formatted, is not valid
                if (serverBaseAddressUri.HostNameType == UriHostNameType.Unknown)
                {
                    Uri newBaseAddressUri = new Uri(bindingScheme + "://" + serverBaseAddressUri);
                    if (newBaseAddressUri.HostNameType != UriHostNameType.Unknown)
                    {
                        serverBaseAddressUri = newBaseAddressUri;
                    }
                    else
                    {
                        throw new ApplicationException("Invalid end point address: " + serverBaseAddress);
                    }
                }

                // Verifies if the pattern of the address has subfolders, adjusts the string to it and removes the sufix slash if exists
                string pathAndQuery = serverBaseAddressUri.PathAndQuery;
                if (pathAndQuery == "/")
                {
                    pathAndQuery = string.Empty;
                }

                // Verifies if the address has a port set to it and adjusts the string to add the port number
                string resultBaseAddress = string.Empty;

                if (serverBaseAddressUri.Port == -1)
                {
                    resultBaseAddress = serverBaseAddressUri.DnsSafeHost;
                }
                else
                {
                    resultBaseAddress = serverBaseAddressUri.DnsSafeHost + ":" + serverBaseAddressUri.Port;
                }

                // Verifies if the pattern of the slashes are correct and fix them if not
                resultBaseAddress = resultBaseAddress.Replace("//", "/");
                if (resultBaseAddress.Substring(resultBaseAddress.Length - 1, 1) == "/")
                {
                    resultBaseAddress = resultBaseAddress.Substring(0, resultBaseAddress.Length - 1);
                }

                // Performs a final verifications on the string (including a regex pattern check) before generating the End Point Address instance
                string innerServiceName = serviceName.Replace("//", "");

                if (pathAndQuery == string.Empty)
                {
                    pathAndQuery = "/";
                }

                if (innerServiceName[0] == '/')
                {
                    innerServiceName = innerServiceName.Substring(1);
                }

                string fullAddress = bindingScheme + "://" + resultBaseAddress + pathAndQuery + innerServiceName;

                Match regexResult = Regex.Match(fullAddress, PATH_CHECK_REGEX);
                if (!regexResult.Success)
                {
                    throw new ApplicationException("Invalid end point address: " + serverBaseAddress);
                }

                // Replaces the current End Point Address of the context with the one generated
                endPointAddress = new EndpointAddress(fullAddress);
            }
            else
            {
                // Uses the current End Point Address of the context
                endPointAddress = Endpoint.Address;
            }

            return endPointAddress;
        }

        /// <summary>
        /// Closes the channel safely.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="channel">The channel.</param>
        /// <exception cref="System.ArgumentNullException">channel</exception>
        public static void SafelyCloseChannel<TChannel, TInterface>(TChannel channel)
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            if (channel == null) throw new ArgumentNullException("channel");

            try
            {
                if (channel.State == CommunicationState.Faulted)
                {
                    channel.Abort();
                }
            }
            finally
            {
                channel.Close();
                if (channel is IDisposable)
                {
                    ((IDisposable)channel).Dispose();
                }
            }
        }

        /// <summary>
        /// Resolves the channel creation factory.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="kernel">The kernel.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">kernel</exception>
        public static IChannelCreationFactory<TChannel, TInterface> ResolveChannelCreationFactory<TChannel, TInterface>(IKernel kernel)
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            try
            {
                return kernel.Resolve<IChannelCreationFactory<TChannel, TInterface>>();
            }
            catch (ComponentResolutionException)
            {
                kernel.Register(Component.For<IChannelCreationFactory<TChannel, TInterface>>().AsFactory());
                return kernel.Resolve<IChannelCreationFactory<TChannel, TInterface>>();
            }
        }

        /// <summary>
        /// A cache for reflection-based channel getters
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        private static class TChannelReflectionInfo<TChannel, TInterface>
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            public static MethodInfo GetMethod { get; private set; }

            static TChannelReflectionInfo()
            {
                GetMethod = typeof(TChannel)
                    .GetProperty("Channel", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetGetMethod(true);
            }
        }

        /// <summary>
        /// Gets the channel's channel.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> GetChannelChannel<TChannel, TInterface>(TChannel channel)
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            var result = TChannelReflectionInfo<TChannel, TInterface>.GetMethod.Invoke(channel, null);
            yield return (TInterface)result;
        }

        /// <summary>
        /// Gets the delegate invoke arguments.
        /// </summary>
        /// <typeparam name="TChannel">The type of the channel.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="channel">The channel.</param>
        /// <param name="invocationArguments">The invocation arguments.</param>
        /// <returns></returns>
        public static object[] GetDelegateInvokeArguments<TChannel, TInterface>(TChannel channel, object[] invocationArguments)
            where TChannel : ClientBase<TInterface>
            where TInterface : class
        {
            return GetChannelChannel<TChannel, TInterface>(channel)
                .Concat(invocationArguments)
                .ToArray();
        }
    }
}
