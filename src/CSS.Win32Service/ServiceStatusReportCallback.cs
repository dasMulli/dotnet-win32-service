namespace CSS.Win32Service
{
    public delegate void ServiceStatusReportCallback(ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, int win32ExitCode, uint waitHint);
}