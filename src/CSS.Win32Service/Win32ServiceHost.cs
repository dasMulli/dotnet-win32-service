using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSS.ServiceHost
{
    // This implementation is roughly based on https://msdn.microsoft.com/en-us/library/bb540475(v=vs.85).aspx
    public sealed class Win32ServiceHost
    {
        private readonly IWin32Service service;
        private string serviceName;

        private ServiceStatus serviceStatus = new ServiceStatus(ServiceType.Win32OwnProcess, ServiceState.StartPening, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, serviceSpecificExitCode: 0, checkPoint: 0, waitHint: 0);

        private ServiceStatusHandle serviceStatusHandle = new ServiceStatusHandle();

        private ManualResetEvent stopEvent = new ManualResetEvent(initialState: false);

        public Win32ServiceHost(IWin32Service service)
        {
            this.service = service;
        }
        
        public void Run()
        {
            serviceName = service.ServiceName; // only query it once

            var serviceTable = new ServiceTableEntry[2]; // second one is null/null
            serviceTable[0].serviceName = serviceName;
            serviceTable[0].serviceMainFunction = Marshal.GetFunctionPointerForDelegate<ServiceMainFunction>(ServiceMainFunction);

            // StartServiceCtrlDispatcherW call returns when ServiceMainFunction exits
            if (!Interop.StartServiceCtrlDispatcherW(serviceTable))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void ServiceMainFunction(uint numArs, IntPtr firstArg)
        {
            serviceStatusHandle = Interop.RegisterServiceCtrlHandlerExW(serviceName, ServiceControlHandler, IntPtr.Zero);
            
            if (serviceStatusHandle.IsInvalid)
            {
                return;
            }

            stopEvent = new ManualResetEvent(initialState: false);
            
            ReportServiceStatus(ServiceState.StartPening, win32ExitCode: 0, waitHint: 3000);
            
            service.Start();
            
            ReportServiceStatus(ServiceState.Running, win32ExitCode: 0, waitHint: 0);

            // wait here
            stopEvent.WaitOne();

            service.Stop();

            ReportServiceStatus(ServiceState.Stopped, win32ExitCode: 0, waitHint: 0);
        }

        private uint checkpointCounter = 1;

        private void ServiceControlHandler(ServiceControlCommand command, uint eventType, IntPtr eventData, IntPtr eventContext)
        {
            if (command == ServiceControlCommand.Stop)
            {
                ReportServiceStatus(ServiceState.StopPending, win32ExitCode: 0, waitHint: 1000);

                stopEvent.Set();
            }
        }

        private void ReportServiceStatus(ServiceState state, uint win32ExitCode, uint waitHint)
        {
            serviceStatus.State = state;
            serviceStatus.Win32ExitCode = win32ExitCode;
            serviceStatus.WaitHint = waitHint;

            serviceStatus.AcceptedControlCommands = state == ServiceState.StartPening
                ? ServiceAcceptedControlCommandsFlags.None
                : ServiceAcceptedControlCommandsFlags.Stop;

            serviceStatus.CheckPoint = state == ServiceState.Running || state == ServiceState.Stopped
                ? 0
                : checkpointCounter++;

            Interop.SetServiceStatus(serviceStatusHandle, ref serviceStatus);
        }
    }
}
