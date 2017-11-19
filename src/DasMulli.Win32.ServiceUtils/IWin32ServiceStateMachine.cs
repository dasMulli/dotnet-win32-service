using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Interface to implement advanced services through custom handling all service state events.
    /// 
    /// See <see cref="SimpleServiceStateMachine"/> for a sample implementation.
    /// </summary>
    [PublicAPI]
    public interface IWin32ServiceStateMachine
    {
        /// <summary>
        /// Called when the service is started.
        /// Use the provided <paramref name="statusReportCallback"/> to notify the service manager about
        /// state changes such as started, paused etc.
        /// </summary>
        /// <param name="startupArguments">The startup arguments passed via windows' service configuration.</param>
        /// <param name="statusReportCallback">The status report callback.</param>
        void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback);

        /// <summary>
        /// Called when a command was received from windows' service system.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="commandSpecificEventType">Type of the command specific event. See description of dwEventType at https://msdn.microsoft.com/en-us/library/windows/desktop/ms683241(v=vs.85).aspx </param>
        void OnCommand(ServiceControlCommand command, uint commandSpecificEventType);
    }
}