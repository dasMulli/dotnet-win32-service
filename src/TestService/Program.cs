using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CSS.ServiceHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TestService
{
    public class Program
    {
        private const string RunAsServiceFlag = "--run-as-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";
        private const string InteractiveFlag = "--interactive";

        private const string ServiceName = "Demo Service";
        private const string ServiceDescription = "Demo ASP.NET Core on .NET Core Service";

        public static void Main(string[] args)
        {
            try
            {
                if (args.Contains(RunAsServiceFlag))
                {
                    var testService = new TestWin32Service(args.Where(a => a != RunAsServiceFlag).ToArray());
                    var serviceHost = new Win32ServiceHost(testService);
                    serviceHost.Run();
                }
                else if (args.Contains(RegisterServiceFlag))
                {
                    // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
                    var remainingArgs = Environment.GetCommandLineArgs()
                        .Where(arg => arg != RegisterServiceFlag)
                        .Select(EscapeCommandLineArgument)
                        .Append(RunAsServiceFlag);

                    var fullServiceCommand = Process.GetCurrentProcess().MainModule.FileName + " " + string.Join(" ", remainingArgs);

                    // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
                    new Win32ServiceManager()
                        .CreateService(ServiceName, ServiceDescription, fullServiceCommand, Win32ServiceCredentials.LocalSystem, autoStart: true, startImmediately: true);

                    Console.WriteLine($@"Sucessfully registered and started service ""{ServiceDescription}""");
                }
                else if (args.Contains(UnregisterServiceFlag))
                {
                    new Win32ServiceManager()
                        .DeleteService(ServiceName);

                    Console.WriteLine($@"Sucessfully unregistered service ""{ServiceDescription}""");
                }
                else if (args.Contains(InteractiveFlag))
                {
                    var testService = new TestWin32Service(args.Where(a => a != InteractiveFlag).ToArray());
                    testService.Start();
                    Console.WriteLine("Running interactively, press enter to stop.");
                    Console.ReadLine();
                    testService.Stop();
                }
                else
                {
                    Console.WriteLine(ServiceDescription);
                    Console.WriteLine();
                    Console.WriteLine("This demo application is intened to be run as windows service. Use one of the following options:");
                    Console.WriteLine("  --register-service        Registers and starts this program as a windows service named \"" + ServiceDescription + "\"");
                    Console.WriteLine("                            All additional arguments will be passed to ASP.NET Core's WebHostBuilder.");
                    Console.WriteLine("  --unregister-service      Removes the windows service creatd by --register-service.");
                    Console.WriteLine("  --interactive             Runs the underlying asp.net core app. Useful to test arguments.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }

        private static string EscapeCommandLineArgument(string arg)
        {
            // http://stackoverflow.com/a/6040946/784387
            arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
            arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
            return arg;
        }
    }

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
