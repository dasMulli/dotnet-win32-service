namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// <para>
    /// This is a callback type passed to custom implementation of Windows service state machines.
    /// The callback needs to be called to notify Windows about service state changes both when requested to
    /// perform state changes by Windows or when they are needed because of other reasons (e.g. unexpected termination).
    /// </para>
    /// <para>
    /// Repeatedly calling this callback also prolongs the default timeout for pending states until the service manager reports the service as failed.
    /// </para>
    /// <para>
    /// Calling this callback will result in a call to <c>SetServiceStatus</c>. See https://msdn.microsoft.com/en-us/library/windows/desktop/ms686241(v=vs.85).aspx.
    /// </para>
    /// </summary>
    /// <param name="state">The current state of the service.</param>
    /// <param name="acceptedControlCommands">The currently accepted control commands. E.g. when you set the <paramref name="state"/> to <see cref="ServiceState.Running"/>, you can indicate that you support pausing and stopping in this state.</param>
    /// <param name="win32ExitCode">The current Win32 exit code. Use this to indicate a failure when setting the state to <see cref="ServiceState.Stopped"/>.</param>
    /// <param name="waitHint">
    /// The estimated time in milliseconds until a state changing operation finishes.
    /// For example, you can repeatedly call this callback with <paramref name="state"/> set to <see cref="ServiceState.StartPending"/> or <see cref="ServiceState.StartPending"/>
    /// using different values to affect the start/stop progress indicator in service management UI dialogs.
    /// </param>
    public delegate void ServiceStatusReportCallback(ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, int win32ExitCode, uint waitHint);
}