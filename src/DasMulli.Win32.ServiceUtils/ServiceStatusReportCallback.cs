namespace DasMulli.Win32.ServiceUtils
{
    public delegate void ServiceStatusReportCallback(ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, int win32ExitCode, uint waitHint);
}