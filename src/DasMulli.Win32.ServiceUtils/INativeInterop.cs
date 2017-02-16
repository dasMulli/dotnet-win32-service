using System;
using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Keep native entry point name.")]
    internal interface INativeInterop
    {
        bool CloseServiceHandle(IntPtr handle);

        bool StartServiceCtrlDispatcherW(ServiceTableEntry[] serviceTable);

        ServiceStatusHandle RegisterServiceCtrlHandlerExW(string serviceName, ServiceControlHandler serviceControlHandler, IntPtr context);
        
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Matches native signature.")]
        bool SetServiceStatus(ServiceStatusHandle statusHandle, ref ServiceStatus pServiceStatus);

        ServiceControlManager OpenSCManagerW(string machineName, string databaseName, ServiceControlManagerAccessRights dwAccess);

        ServiceHandle CreateServiceW(
            ServiceControlManager serviceControlManager,
            string serviceName,
            string displayName,
            ServiceControlAccessRights desiredControlAccess,
            ServiceType serviceType,
            ServiceStartType startType,
            ErrorSeverity errorSeverity,
            string binaryPath,
            string loadOrderGroup,
            IntPtr outUIntTagId,
            string dependencies,
            string serviceUserName,
            string servicePassword);

        bool ChangeServiceConfigW(
            ServiceHandle service,
            ServiceType serviceType,
            ServiceStartType startType,
            ErrorSeverity errorSeverity,
            string binaryPath,
            string loadOrderGroup,
            IntPtr outUIntTagId,
            string dependencies,
            string serviceUserName,
            string servicePassword,
            string displayName);

        ServiceHandle OpenServiceW(ServiceControlManager serviceControlManager, string serviceName, ServiceControlAccessRights desiredControlAccess);

        bool StartServiceW(ServiceHandle service, uint argc, IntPtr wargv);

        bool DeleteService(ServiceHandle service);

        bool ChangeServiceConfig2W(ServiceHandle service, ServiceConfigInfoTypeLevel infoTypeLevel, IntPtr info);
    }
}