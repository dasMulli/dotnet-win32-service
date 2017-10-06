using System;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MvcTestService
{
    public class MvcTestWin32Service : IWin32Service
    {
        public string ServiceName => "MVC Sample Service";

        private readonly string[] commandLineArguments;
        private IWebHost webHost;
        private bool stopRequestedByWindows;

        public MvcTestWin32Service(string[] commandLineArguments)
        {
            this.commandLineArguments = commandLineArguments;
        }

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

            webHost = Program.BuildWebHost(combinedArguments);
            
            // Make sure the windows service is stopped if the
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
