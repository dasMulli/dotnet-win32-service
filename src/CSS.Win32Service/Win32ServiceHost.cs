using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CSS.Win32Service
{
    // This implementation is roughly based on https://msdn.microsoft.com/en-us/library/bb540475(v=vs.85).aspx
    [PublicAPI]
    public sealed class Win32ServiceHost
    {
        private readonly string serviceName;
        private readonly IWin32ServiceStateMachine stateMachine;

        private ServiceStatus serviceStatus = new ServiceStatus(ServiceType.Win32OwnProcess, ServiceState.StartPening, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, serviceSpecificExitCode: 0, checkPoint: 0, waitHint: 0);
        private ServiceStatusHandle serviceStatusHandle;

        private readonly TaskCompletionSource<int> stopTaskCompletionSource = new TaskCompletionSource<int>();

        public Win32ServiceHost(IWin32Service service)
            : this(service.ServiceName, new SimpleServiceStateMachine(service))
        {
        }

        public Win32ServiceHost(string serviceName, IWin32ServiceStateMachine stateMachine)
        {
            this.serviceName = serviceName;
            this.stateMachine = stateMachine;
        }

        public Task<int> RunAsync()
        {
            var serviceTable = new ServiceTableEntry[2]; // second one is null/null to indicate termination
            serviceTable[0].serviceName = serviceName;
            serviceTable[0].serviceMainFunction = Marshal.GetFunctionPointerForDelegate<ServiceMainFunction>(ServiceMainFunction);

            try
            {
                // StartServiceCtrlDispatcherW call returns when ServiceMainFunction exits
                if (!Interop.StartServiceCtrlDispatcherW(serviceTable))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.", dllException);
            }
            
            return stopTaskCompletionSource.Task;
        }

        public int Run()
        {
            return RunAsync().Result;
        }

        private void ServiceMainFunction(uint numArs, IntPtr firstArg)
        {
            serviceStatusHandle = Interop.RegisterServiceCtrlHandlerExW(serviceName, HandleServiceControlCommand, IntPtr.Zero);

            if (serviceStatusHandle.IsInvalid)
            {
                stopTaskCompletionSource.SetException(new Win32Exception(Marshal.GetLastWin32Error()));
                return;
            }

            ReportServiceStatus(ServiceState.StartPening, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

            try
            {
                stateMachine.OnStart(ReportServiceStatus);
            }
            catch
            {
                ReportServiceStatus(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1, waitHint: 0);
            }
        }

        private uint checkpointCounter = 1;

        private void ReportServiceStatus(ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, int win32ExitCode, uint waitHint)
        {
            if (serviceStatus.State == ServiceState.Stopped)
            {
                // we refuse to leave or alter the final state
                return;
            }

            serviceStatus.State = state;
            serviceStatus.Win32ExitCode = win32ExitCode;
            serviceStatus.WaitHint = waitHint;
            
            serviceStatus.AcceptedControlCommands = state == ServiceState.Stopped 
                ? ServiceAcceptedControlCommandsFlags.None // since we enforce "Stopped" as final state, no longer accept control messages
                : acceptedControlCommands;

            serviceStatus.CheckPoint = state == ServiceState.Running || state == ServiceState.Stopped || state == ServiceState.Paused
                ? 0 // MSDN: This value is not valid and should be zero when the service does not have a start, stop, pause, or continue operation pending.
                : checkpointCounter++;

            Interop.SetServiceStatus(serviceStatusHandle, ref serviceStatus);

            if (state == ServiceState.Stopped)
            {
                stopTaskCompletionSource.TrySetResult(win32ExitCode);
            }
        }

        private void HandleServiceControlCommand(ServiceControlCommand command, uint eventType, IntPtr eventData, IntPtr eventContext)
        {
            try
            {
                stateMachine.OnCommand(command, eventType);
            }
            catch
            {
                ReportServiceStatus(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1,  waitHint: 0);
            }
        }
    }
}
