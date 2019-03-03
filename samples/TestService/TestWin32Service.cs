using System;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestService
{
    internal class TestWin32Service : IWin32Service
    {
        private readonly string[] commandLineArguments;
        private IWebHost webHost;
        private bool stopRequestedByWindows;

        public TestWin32Service(string[] commandLineArguments)
        {
            this.commandLineArguments = commandLineArguments;
        }

        public string ServiceName => "Test Service";

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            // in addition to the arguments that the service has been registered with,
            // each service start may add additional startup parameters.
            // To test this: Open services console, open service details, enter startup arguments and press start.
            string[] combinedArguments;
            if (startupArguments.Length > 0)
            {
                combinedArguments = new string[commandLineArguments.Length + startupArguments.Length];
                Array.Copy(commandLineArguments, combinedArguments, commandLineArguments.Length);
                Array.Copy(startupArguments, 0, combinedArguments, commandLineArguments.Length, startupArguments.Length);
            }
            else
            {
                combinedArguments = commandLineArguments;
            }

            var config = new ConfigurationBuilder()
                .AddCommandLine(combinedArguments)
                .Build();

            webHost = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<AspNetCoreStartup>()
                .UseConfiguration(config)
                .Build();

            // Make sure the Windows service is stopped if the
            // ASP.NET Core stack stops for any reason
            webHost
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

            webHost.Start();
        }

        public void Stop()
        {
            stopRequestedByWindows = true;
            webHost.Dispose();
        }
    }
}