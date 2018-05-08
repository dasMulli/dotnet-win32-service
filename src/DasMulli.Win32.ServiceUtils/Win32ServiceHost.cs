using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    // This implementation is roughly based on https://msdn.microsoft.com/en-us/library/bb540475(v=vs.85).aspx
    /// <summary>
    /// Runs windows services as requested by windows' service management.
    /// Use this class as a replacement for ServiceBase.Run()
    /// </summary>
    [PublicAPI]
    public sealed class Win32ServiceHost
    {
        private readonly string serviceName;
        private readonly IWin32ServiceStateMachine stateMachine;
        private readonly INativeInterop nativeInterop;

        private readonly ServiceMainFunction serviceMainFunctionDelegate;
        private readonly ServiceControlHandler serviceControlHandlerDelegate;

        private ServiceStatus serviceStatus = new ServiceStatus(ServiceType.Win32OwnProcess, ServiceState.StartPending, ServiceAcceptedControlCommandsFlags.None,
            win32ExitCode: 0, serviceSpecificExitCode: 0, checkPoint: 0, waitHint: 0);

        private ServiceStatusHandle serviceStatusHandle;

        private int resultCode;
        private Exception resultException;

        /// <summary>
        /// Initializes a new <see cref="Win32ServiceHost"/> to run the specified windows service implementation.
        /// </summary>
        /// <param name="service">The windows service implementation about to be run.</param>
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

            this.nativeInterop = nativeInterop ?? throw new ArgumentNullException(nameof(nativeInterop));
            serviceName = service.ServiceName;
            stateMachine = new SimpleServiceStateMachine(service);

            serviceMainFunctionDelegate = ServiceMainFunction;
            serviceControlHandlerDelegate = HandleServiceControlCommand;
        }

        /// <summary>
        /// Initializes a new <see cref="IPausableWin32Service"/> to run the specified windows service implementation.
        /// </summary>
        /// <param name="service">The windows service implementation about to be run.</param>
        public Win32ServiceHost([NotNull] IPausableWin32Service service): this(service, Win32Interop.Wrapper)
        {
        }

        internal Win32ServiceHost([NotNull] IPausableWin32Service service, [NotNull] INativeInterop nativeInterop)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.nativeInterop = nativeInterop ?? throw new ArgumentNullException(nameof(nativeInterop));
            serviceName = service.ServiceName;
            stateMachine = new PausableServiceStateMachine(service);

            serviceMainFunctionDelegate = ServiceMainFunction;
            serviceControlHandlerDelegate = HandleServiceControlCommand;
        }

        /// <summary>
        /// Initializes a new <see cref="Win32ServiceHost"/> class to run an advanced service with custom state handling.
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        /// <param name="stateMachine">The custom service state machine implementation to use.</param>
        public Win32ServiceHost([NotNull] string serviceName, [NotNull] IWin32ServiceStateMachine stateMachine)
            : this(serviceName, stateMachine, Win32Interop.Wrapper)
        {
        }

        internal Win32ServiceHost([NotNull] string serviceName, [NotNull] IWin32ServiceStateMachine stateMachine, [NotNull] INativeInterop nativeInterop)
        {
            this.serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            this.nativeInterop = nativeInterop ?? throw new ArgumentNullException(nameof(nativeInterop));

            serviceMainFunctionDelegate = ServiceMainFunction;
            serviceControlHandlerDelegate = HandleServiceControlCommand;
        }

        /// <summary>
        /// Obsolete - pretends to run asynchronously.
        /// Only exists for compatibility with 1.0 API
        /// </summary>
        /// <returns></returns>
        [Obsolete("Doesn't really work when used in an async continuation on a background thread due to windows API requirements. Use Run() from the main thread instead (blocking).")]
        [EditorBrowsable(EditorBrowsableState.Never)]
#if NETSTANDARD2_0
        [Browsable(false)]
#endif
        public Task<int> RunAsync() => Task.FromResult(Run());

        /// <summary>
        /// Runs the windows service that this instance was initialized with.
        /// This method is inteded to be run from the application's main thread and will block until the service has stopped.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Win32Exception">Thrown when an exception ocurrs when communicating with windows' service system.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when run on a non-windows platform.</exception>
        public int Run()
        {
            var serviceTable = new ServiceTableEntry[2]; // second one is null/null to indicate termination
            serviceTable[0].serviceName = serviceName;
            serviceTable[0].serviceMainFunction = Marshal.GetFunctionPointerForDelegate(serviceMainFunctionDelegate);

            try
            {
                // StartServiceCtrlDispatcherW call returns when ServiceMainFunction has exited and all services have stopped
                // at least this is what's documented even though linked c++ sample has an additional stop event
                // to let the service main function dispatched to block until the service stops.
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

            if (resultException != null)
            {
                throw resultException;
            }

            return resultCode;
        }

        private void ServiceMainFunction(int numArgs, IntPtr argPtrPtr)
        {
            var startupArguments = ParseArguments(numArgs, argPtrPtr);

            serviceStatusHandle = nativeInterop.RegisterServiceCtrlHandlerExW(serviceName, serviceControlHandlerDelegate, IntPtr.Zero);

            if (serviceStatusHandle.IsInvalid)
            {
                resultException = new Win32Exception(Marshal.GetLastWin32Error());
                return;
            }

            ReportServiceStatus(ServiceState.StartPending, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

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

            if (state == ServiceState.Stopped)
            {
                resultCode = win32ExitCode;
            }

            nativeInterop.SetServiceStatus(serviceStatusHandle, ref serviceStatus);
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