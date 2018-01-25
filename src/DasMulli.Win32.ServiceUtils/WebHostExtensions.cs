using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DasMulli.Win32.ServiceUtils
{
    public static class WebHostExtensions
    {
        public static void RunAsService(this IWebHost webhost, string serviceName) =>
            new Win32ServiceHost(new DummyService(webhost, serviceName)).Run();
    }

    internal class DummyService : IWin32Service
    {
        private readonly IWebHost webHost;
        public bool stopRequestedByWindows { get; private set; }
        public string ServiceName { get; }

        public DummyService(IWebHost webHost, string serviceName)
        {
            this.ServiceName = serviceName;
            this.webHost = webHost;
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
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