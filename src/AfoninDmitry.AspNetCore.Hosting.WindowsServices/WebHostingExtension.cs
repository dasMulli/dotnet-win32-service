using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore.Hosting;

namespace AfoninDmitry.AspNetCore.Hosting.WindowsServices
{
    public static class WebHostingExtension
    {
        public static void RunAsService(this IWebHost webhost, string serviceName) => 
            new Win32ServiceHost(new DummyService(webhost, serviceName)).Run();
    }
}