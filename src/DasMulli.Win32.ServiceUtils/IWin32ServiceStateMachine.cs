using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [PublicAPI]
    public interface IWin32ServiceStateMachine
    {
        void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback);

        void OnCommand(ServiceControlCommand command, uint commandSpecificEventType);
    }
}