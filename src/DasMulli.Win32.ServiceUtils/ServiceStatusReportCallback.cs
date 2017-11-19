namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// This is a callback type passed to custom implementaton of windows service state machines.
    /// The callback needs to be called to notify windows about service state changes both when requested to
    /// perform state changes by windows or when they are needed because of other reasons (e.g. unexpected termination).
    /// 
    /// Repeatedly calling this callback also prolonges the default timeout for pending states until the service maanger reports the service as failed.
    /// 
    /// Calling this callback will result in a call to SetServiceStatus - see https://msdn.microsoft.com/en-us/library/windows/desktop/ms686241(v=vs.85).aspx
    /// </summary>
    /// <param name="state">The current state of the service.</param>
    /// <param name="acceptedControlCommands">The currently accepted control commands. E.g. when you set the <paramref name="state"/> to <value>Started</value>, you can indicate that you support pausing and stopping in this state.</param>
    /// <param name="win32ExitCode">The  current win32 exit code. Use this to indicate a failure when setting the state to <value>Stopped</value>.</param>
    /// <param name="waitHint">
    /// The estimeated time in milliseconds until a state changing operation finishes.
    /// For example, you can repeatedly call this callback with <paramref name="state"/> set to <value>StartPending</value> or <value>StopPending</value>
    /// using different values to affect the start/stop progress indicator in service management UI dialogs.
    /// </param>
    public delegate void ServiceStatusReportCallback(ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, int win32ExitCode, uint waitHint);
}