namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Callback type that is used to allow implementations of <see cref="IWin32Service"/> to notify
    /// the service manager in case it stopped without being requested to stop.
    /// </summary>
    public delegate void ServiceStoppedCallback();

    /// <summary>.
    /// Represents a simple Windows service that can start and stop.
    /// </summary>
    public interface IWin32Service
    {
        /// <summary>
        /// Returns the name of the service.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Called by the service host to start the service. When called by <see cref="Win32ServiceHost"/>,
        /// the service startup arguments received from Windows are specified.
        /// </summary>
        /// <param name="startupArguments">The startup arguments.</param>
        /// <param name="serviceStoppedCallback">Notifies the service manager that the service stopped without being requested to stop.</param>
        void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback);

        /// <summary>
        /// Called by the service host to stop the service.
        /// </summary>
        void Stop();
    }
}