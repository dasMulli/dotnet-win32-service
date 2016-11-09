using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    // This implementation is roughly based on https://msdn.microsoft.com/en-us/library/bb540475(v=vs.85).aspx
    [PublicAPI]
    public sealed class Win32ServiceHost
    {
        private readonly string serviceName;
        private readonly IWin32ServiceStateMachine stateMachine;
        private readonly INativeInterop nativeInterop;

        private readonly ServiceMainFunction serviceMainFunctionDelegate;
        private readonly ServiceControlHandler serviceControlHandlerDelegate;

        private ServiceStatus serviceStatus = new ServiceStatus(ServiceType.Win32OwnProcess, ServiceState.StartPening, ServiceAcceptedControlCommandsFlags.None,
            win32ExitCode: 0, serviceSpecificExitCode: 0, checkPoint: 0, waitHint: 0);

        private ServiceStatusHandle serviceStatusHandle;

        private readonly TaskCompletionSource<int> stopTaskCompletionSource = new TaskCompletionSource<int>();

        public Win32ServiceHost([NotNull] IWin32Service service)
            : this(service, Win32Interop.Wrapper)
        {
        }

        internal Win32ServiceHost([NotNull] IWin32Service service, [NotNull] INativeInterop nativeInterop)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (nativeInterop == null)
            {
                throw new ArgumentNullException(nameof(nativeInterop));
            }

            serviceName = service.ServiceName;
            stateMachine = new SimpleServiceStateMachine(service);
            this.nativeInterop = nativeInterop;

            serviceMainFunctionDelegate = ServiceMainFunction;
            serviceControlHandlerDelegate = HandleServiceControlCommand;
        }

        public Win32ServiceHost([NotNull] string serviceName, [NotNull] IWin32ServiceStateMachine stateMachine)
            : this(serviceName, stateMachine, Win32Interop.Wrapper)
        {
        }

        internal Win32ServiceHost([NotNull] string serviceName, [NotNull] IWin32ServiceStateMachine stateMachine, [NotNull] INativeInterop nativeInterop)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }
            if (nativeInterop == null)
            {
                throw new ArgumentNullException(nameof(nativeInterop));
            }

            this.serviceName = serviceName;
            this.stateMachine = stateMachine;
            this.nativeInterop = nativeInterop;

            serviceMainFunctionDelegate = ServiceMainFunction;
            serviceControlHandlerDelegate = HandleServiceControlCommand;
        }

        public Task<int> RunAsync()
        {
            var serviceTable = new ServiceTableEntry[2]; // second one is null/null to indicate termination
            serviceTable[0].serviceName = serviceName;
            serviceTable[0].serviceMainFunction = Marshal.GetFunctionPointerForDelegate(serviceMainFunctionDelegate);

            try
            {
                // StartServiceCtrlDispatcherW call returns when ServiceMainFunction exits
                if (!nativeInterop.StartServiceCtrlDispatcherW(serviceTable))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.",
                    dllException);
            }

            return stopTaskCompletionSource.Task;
        }

        public int Run()
        {
            return RunAsync().Result;
        }

        private void ServiceMainFunction(int numArgs, IntPtr argPtrPtr)
        {
            var startupArguments = ParseArguments(numArgs, argPtrPtr);

            serviceStatusHandle = nativeInterop.RegisterServiceCtrlHandlerExW(serviceName, serviceControlHandlerDelegate, IntPtr.Zero);

            if (serviceStatusHandle.IsInvalid)
            {
                stopTaskCompletionSource.SetException(new Win32Exception(Marshal.GetLastWin32Error()));
                return;
            }

            ReportServiceStatus(ServiceState.StartPening, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

            try
            {
                stateMachine.OnStart(startupArguments, ReportServiceStatus);
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

            nativeInterop.SetServiceStatus(serviceStatusHandle, ref serviceStatus);

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
                ReportServiceStatus(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1, waitHint: 0);
            }
        }

        private static string[] ParseArguments(int numArgs, IntPtr argPtrPtr)
        {
            if (numArgs <= 0)
            {
                return Array.Empty<string>();
            }
            // skip first parameter becuase it is the name of the service
            var args = new string[numArgs - 1];
            for (var i = 0; i < numArgs - 1; i++)
            {
                argPtrPtr = IntPtr.Add(argPtrPtr, IntPtr.Size);
                var argPtr = Marshal.PtrToStructure<IntPtr>(argPtrPtr);
                args[i] = Marshal.PtrToStringUni(argPtr);
            }
            return args;
        }
    }
}