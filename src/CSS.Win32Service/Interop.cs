using System;
using System.Runtime.InteropServices;

namespace CSS.ServiceHost
{
    internal static class Interop
    {
        private const string DllServiceCore_L1_1_0 = "api-ms-win-service-core-l1-1-0.dll";
        private const string DllServiceManagement_L1_1_0 = "api-ms-win-service-management-l1-1-0.dll";

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true)]
        internal static extern bool CloseServiceHandle(IntPtr handle);

        [DllImport(DllServiceCore_L1_1_0, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool StartServiceCtrlDispatcherW([MarshalAs(UnmanagedType.LPArray)] ServiceTableEntry[] serviceTable);

        [DllImport(DllServiceCore_L1_1_0, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ServiceStatusHandle RegisterServiceCtrlHandlerExW(string serviceName, ServiceControlHandler serviceControlHandler, IntPtr context);

        [DllImport(DllServiceCore_L1_1_0, ExactSpelling = true, SetLastError = true)]
        internal static extern bool SetServiceStatus(ServiceStatusHandle statusHandle, ref ServiceStatus pServiceStatus);

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ServiceControlManager OpenSCManagerW(string machineName, string databaseName, ServiceControlManagerAccessRights dwAccess);

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ServiceHandle CreateServiceW(
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

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern ServiceHandle OpenServiceW(ServiceControlManager serviceControlManager, string serviceName, ServiceControlAccessRights desiredControlAccess);

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true)]
        internal static extern bool StartServiceW(ServiceHandle service, uint argc, IntPtr wargv);

        [DllImport(DllServiceManagement_L1_1_0, ExactSpelling = true, SetLastError = true)]
        internal static extern bool DeleteService(ServiceHandle service);
    }
}