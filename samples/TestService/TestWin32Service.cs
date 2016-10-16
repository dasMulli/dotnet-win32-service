using CSS.Win32Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TestService
{
    internal class TestWin32Service : IWin32Service
    {
        private readonly string[] commandLineArguments;
        private IWebHost webHost;

        public TestWin32Service(string[] commandLineArguments)
        {
            this.commandLineArguments = commandLineArguments;
        }

        public string ServiceName => "Test Service";

        public void Start()
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(commandLineArguments)
                .Build();

            webHost = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<AspNetCoreStartup>()
                .UseConfiguration(config)
                .Build();

            webHost.Start();
        }

        public void Stop()
        {
            webHost.Dispose();
        }
    }
}