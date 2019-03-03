using System;
using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Implements the state machine to handle a simple service that only implement starting and stopping.
    /// These simple services are implemented by configuring to the <see cref="IWin32Service"/> protocol.
    /// </summary>
    /// <seealso cref="IWin32ServiceStateMachine" />
    public sealed class PausableServiceStateMachine : IWin32ServiceStateMachine
    {
        private readonly IPausableWin32Service serviceImplementation;
        private ServiceStatusReportCallback statusReportCallback;

        /// <summary>
        /// Initializes a new <see cref="PausableServiceStateMachine"/> to run the specified service.
        /// </summary>
        /// <param name="serviceImplementation">The service implementation.</param>
        public PausableServiceStateMachine(IPausableWin32Service serviceImplementation)
        {
            this.serviceImplementation = serviceImplementation;
        }

        /// <summary>
        /// Called by the service host to start the service. When called by <see cref="Win32ServiceHost"/>,
        /// the service startup arguments received from Windows are specified.
        /// Use the provided <see cref="ServiceStatusReportCallback"/> to notify the service manager about
        /// state changes such as started, paused etc.
        /// </summary>
        /// <param name="startupArguments">The startup arguments.</param>
        /// <param name="statusReportCallback">Notifies the service manager of a status change.</param>
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback)
        {
            this.statusReportCallback = statusReportCallback;

            try
            {
                serviceImplementation.Start(startupArguments, HandleServiceImplementationStoppedOnItsOwn);

                statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.PauseContinueStop, win32ExitCode: 0, waitHint: 0);
            }
            catch
            {
                statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1, waitHint: 0);
            }
        }

        /// <summary>
        /// Called by the service host to start the service. When called by <see cref="Win32ServiceHost"/>,
        /// the service startup arguments received from Windows are specified.
        /// Use the provided <see cref="ServiceStatusReportCallback"/> to notify the service manager about
        /// state changes such as started, paused etc.
        /// </summary>
        /// <param name="startupArguments">The startup arguments.</param>
        /// <param name="statusReportCallback">Notifies the service manager of a status change.</param>
        public void OnCommand(ServiceControlCommand command, uint commandSpecificEventType)
        {
            switch (command)
            {
                case ServiceControlCommand.Stop:
                    PerformAction(ServiceState.StopPending, ServiceState.Stopped, serviceImplementation.Stop, ServiceAcceptedControlCommandsFlags.None);
                    break;
                case ServiceControlCommand.Pause:
                    PerformAction(ServiceState.PausePending, ServiceState.Paused, serviceImplementation.Pause, ServiceAcceptedControlCommandsFlags.PauseContinueStop);
                    break;
                case ServiceControlCommand.Continue:
                    PerformAction(ServiceState.ContinuePending, ServiceState.Running, serviceImplementation.Continue, ServiceAcceptedControlCommandsFlags.PauseContinueStop);
                    break;
            }
        }

        private void PerformAction(ServiceState pendingState, ServiceState completedState, Action serviceAction, ServiceAcceptedControlCommandsFlags allowedControlCommandsFlags)
        {
            statusReportCallback(pendingState, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

            try
            {
                serviceAction();
                statusReportCallback(completedState, allowedControlCommandsFlags, 0, waitHint: 0);
            }
            catch
            {
                statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, -1, waitHint: 0);
            }
        }

        private void HandleServiceImplementationStoppedOnItsOwn()
        {
            statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 0);
        }
    }
}