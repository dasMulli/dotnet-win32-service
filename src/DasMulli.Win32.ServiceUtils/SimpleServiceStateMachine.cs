using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    public sealed class SimpleServiceStateMachine : IWin32ServiceStateMachine
    {
        private readonly IWin32Service serviceImplementation;
        private ServiceStatusReportCallback statusReportCallback;

        public SimpleServiceStateMachine(IWin32Service serviceImplementation)
        {
            this.serviceImplementation = serviceImplementation;
        }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback)
        {
            this.statusReportCallback = statusReportCallback;

            try
            {
                serviceImplementation.Start(startupArguments, HandleServiceImplementationStoppedOnItsOwn);

                statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.Stop, win32ExitCode: 0, waitHint: 0);
            }
            catch
            {
                statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: -1, waitHint: 0);
            }
        }

        public void OnCommand(ServiceControlCommand command, uint commandSpecificEventType)
        {
            if (command == ServiceControlCommand.Stop)
            {
                statusReportCallback(ServiceState.StopPending, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 3000);

                var win32ExitCode = 0;

                try
                {
                    serviceImplementation.Stop();
                }
                catch
                {
                    win32ExitCode = -1;
                }

                statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode, waitHint: 0);
            }
        }

        private void HandleServiceImplementationStoppedOnItsOwn()
        {
            statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 0, waitHint: 0);
        }
    }
}