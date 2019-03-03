using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Provides custom handling of service state events.
    /// See <see cref="SimpleServiceStateMachine"/> for a sample implementation.
    /// </summary>
    [PublicAPI]
    public interface IWin32ServiceStateMachine
    {
        /// <summary>
        /// Called by the service host to start the service. When called by <see cref="Win32ServiceHost"/>,
        /// the service startup arguments received from Windows are specified.
        /// Use the provided <see cref="ServiceStatusReportCallback"/> to notify the service manager about
        /// state changes such as started, paused etc.
        /// </summary>
        /// <param name="startupArguments">The startup arguments.</param>
        /// <param name="statusReportCallback">Notifies the service manager of a status change.</param>
        void OnStart(string[] startupArguments, ServiceStatusReportCallback statusReportCallback);

        /// <summary>
        /// Called by the service host when a command was received from Windows' service system.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="commandSpecificEventType">Type of the command specific event. See description of dwEventType at https://msdn.microsoft.com/en-us/library/windows/desktop/ms683241(v=vs.85).aspx </param>
        void OnCommand(ServiceControlCommand command, uint commandSpecificEventType);
    }
}