using System;

namespace CSS.ServiceHost
{
    public sealed class Win32ServiceManager
    {
        private readonly string machineName;
        private readonly string databaseName;
        
        public Win32ServiceManager(string machineName = null, string databaseName = null)
        {
            this.machineName = machineName;
            this.databaseName = databaseName;
        }

        public void CreateService(string serviceName, string displayName, string binaryPath, Win32ServiceCredentials credentials, bool autoStart = false, bool startImmediately = false, ErrorSeverity errorSeverity = ErrorSeverity.Normal)
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

            using (var mgr = ServiceControlManager.Connect(machineName, databaseName, ServiceControlManagerAccessRights.All))
            {
                using (
                    var svc = mgr.CreateService(serviceName, displayName, binaryPath, ServiceType.Win32OwnProcess,
                        autoStart ? ServiceStartType.AutoStart : ServiceStartType.StartOnDemand, errorSeverity, credentials))
                {
                    if (startImmediately)
                    {
                        svc.Start();
                    }
                }
            }
        }

        public void DeleteService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serviceName));
            }

            using (var mgr = ServiceControlManager.Connect(machineName, databaseName, ServiceControlManagerAccessRights.All))
            {
                using (var svc = mgr.OpenService(serviceName, ServiceControlAccessRights.All))
                {
                    svc.Delete();
                }
            }
        }
    }
}
