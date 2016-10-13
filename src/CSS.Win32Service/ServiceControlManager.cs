using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CSS.ServiceHost
{
    internal class ServiceControlManager : SafeHandle
    {
        internal ServiceControlManager() : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.CloseServiceHandle(handle);
        }

        public override bool IsInvalid
        {
            [System.Security.SecurityCritical]
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        internal static ServiceControlManager Connect(string machineName, string databaseName, ServiceControlManagerAccessRights desiredAccessRights)
        {
            var mgr = Interop.OpenSCManagerW(machineName, databaseName, desiredAccessRights);

            if (mgr.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return mgr;
        }

        public ServiceHandle CreateService(string serviceName, string displayName, string binaryPath, ServiceType serviceType, ServiceStartType startupType, ErrorSeverity errorSeverity, Win32ServiceCredentials credentials)
        {
            var service = Interop.CreateServiceW(this, serviceName, displayName, ServiceControlAccessRights.All, serviceType, startupType, errorSeverity,
                binaryPath, null,
                IntPtr.Zero, null, credentials.UserName, credentials.Password);

            if (service.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return service;
        }

        public ServiceHandle OpenService(string serviceName, ServiceControlAccessRights desiredControlAccess)
        {
            var svc = Interop.OpenServiceW(this, serviceName, desiredControlAccess);

            if (svc.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return svc;
        }
    }
}