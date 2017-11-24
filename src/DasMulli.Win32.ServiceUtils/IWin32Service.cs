namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Callback type that is used to allow implementations of <see cref="IWin32Service"/> to notify
    /// the service manager in case it stopped prematurely / without being requested to stop.
    /// </summary>
    public delegate void ServiceStoppedCallback();

    /// <summary>
    /// Interface to allow implementing simple windows services that can start and stop
    /// </summary>
    public interface IWin32Service
    {
        /// <summary>
        /// Returns the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        string ServiceName { get; }

        /// <summary>
        /// Starts the service with the startup arguments received from windows.
        /// </summary>
        /// <param name="startupArguments">The startup arguments.</param>
        /// <param name="serviceStoppedCallback">The service stopped callback the service can call if it stopped without being requested to stop.</param>
        void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback);

        /// <summary>
        /// Stops the service.
        /// </summary>
        void Stop();
    }
}