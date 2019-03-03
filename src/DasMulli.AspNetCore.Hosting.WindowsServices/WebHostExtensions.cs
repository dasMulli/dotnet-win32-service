using DasMulli.Win32.ServiceUtils;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace DasMulli.AspNetCore.Hosting.WindowsServices
{
    /// <summary>
    /// Extensions to <see cref="IWebHost"/> for Windows service hosting scenarios.
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        /// Runs the specified web application inside a Windows service and blocks until the service is stopped.
        /// </summary>
        /// <param name="host">An instance of the <see cref="IWebHost"/> to host in the Windows service.</param>
        /// <param name="serviceName">The name of the service to run.</param>
        [PublicAPI]
        public static void RunAsService(this IWebHost host, string serviceName = null) =>
            new Win32ServiceHost(new WebHostService(host, serviceName)).Run();
    }
}