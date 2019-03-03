using System.Reflection;
using DasMulli.Win32.ServiceUtils;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DasMulli.AspNetCore.Hosting.WindowsServices
{
    /// <inheritdoc />
    /// <summary>
    /// Provides an implementation of a service that hosts an ASP.NET Core application.
    /// </summary>
    /// <seealso cref="T:DasMulli.Win32.ServiceUtils.IWin32Service" />
    [PublicAPI]
    public class WebHostService : IWin32Service
    {
        private readonly IWebHost host;
        private bool stopRequestedByWindows;

        /// <inheritdoc />
        public string ServiceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostService"/> class which hosts the specified host as a Windows service.
        /// </summary>
        /// <param name="host">The host to run as a service.</param>
        /// <param name="serviceName">The name of the service to run. If <see langword="null"/>, the name of the entry assembly is used.</param>
        public WebHostService(IWebHost host, string serviceName = null)
        {
            if (serviceName == null)
            {
                serviceName = Assembly.GetEntryAssembly().GetName().Name;
            }

            ServiceName = serviceName;
            this.host = host;
        }

        /// <inheritdoc />
        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            host
                .Services
                .GetRequiredService<IApplicationLifetime>()
                .ApplicationStopped
                .Register(() =>
                {
                    if (stopRequestedByWindows == false)
                    {
                        serviceStoppedCallback();
                    }
                });

            host.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            stopRequestedByWindows = true;
            host.Dispose();
        }
    }
}