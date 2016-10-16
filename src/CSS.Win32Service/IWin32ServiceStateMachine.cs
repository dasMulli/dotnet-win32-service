using JetBrains.Annotations;

namespace CSS.Win32Service
{
    [PublicAPI]
    public interface IWin32ServiceStateMachine
    {
        void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback);

        void OnCommand(ServiceControlCommand command, uint commandSpecificEventType);
    }
}