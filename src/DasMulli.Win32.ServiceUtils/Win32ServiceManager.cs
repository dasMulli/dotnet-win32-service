using System;

namespace DasMulli.Win32.ServiceUtils
{
    public sealed class Win32ServiceManager
    {
        private readonly string machineName;
        private readonly string databaseName;
        private readonly INativeInterop nativeInterop;

        public Win32ServiceManager(string machineName = null, string databaseName = null)
            : this(machineName, databaseName, Win32Interop.Wrapper)
        {
        }
        
        internal Win32ServiceManager(string machineName, string databaseName, INativeInterop nativeInterop)
        {
            this.machineName = machineName;
            this.databaseName = databaseName;
            this.nativeInterop = nativeInterop;
        }

        public void CreateService(string serviceName, string displayName, string description, string binaryPath, Win32ServiceCredentials credentials, bool autoStart = false, bool startImmediately = false, ErrorSeverity errorSeverity = ErrorSeverity.Normal)
        {
            if (string.IsNullOrEmpty(binaryPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(binaryPath));
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serviceName));
            }
            if (string.IsNullOrEmpty(binaryPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(binaryPath));
            }

            try
            {
                using (var mgr = ServiceControlManager.Connect(nativeInterop, machineName, databaseName, ServiceControlManagerAccessRights.All))
                {
                    using (
                        var svc = mgr.CreateService(serviceName, displayName, binaryPath, ServiceType.Win32OwnProcess,
                            autoStart ? ServiceStartType.AutoStart : ServiceStartType.StartOnDemand, errorSeverity, credentials))
                    {
                        if (!string.IsNullOrEmpty(description))
                        {
                            svc.SetDescription(description);
                        }
                        if (startImmediately)
                        {
                            svc.Start();
                        }
                    }
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.", dllException);
            }
        }

        public void DeleteService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serviceName));
            }

            try
            {
                using (var mgr = ServiceControlManager.Connect(nativeInterop, machineName, databaseName, ServiceControlManagerAccessRights.All))
                {
                    using (var svc = mgr.OpenService(serviceName, ServiceControlAccessRights.All))
                    {
                        svc.Delete();
                    }
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.", dllException);
            }
        }
    }
}
