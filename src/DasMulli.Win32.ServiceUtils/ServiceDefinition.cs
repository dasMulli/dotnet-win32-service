using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [PublicAPI]
    public class ServiceDefinition
    {
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string BinaryPath { get; set; }
        public Win32ServiceCredentials Credentials { get; set; } = Win32ServiceCredentials.LocalSystem;
        public ServiceFailureActions FailureActions { get; set; }
        public bool FailureActionsOnNonCrashFailures { get; set; }
        public bool AutoStart { get; set; } = true;
        public ErrorSeverity ErrorSeverity { get; set; } = ErrorSeverity.Normal;

        public ServiceDefinition(string serviceName, string binPath)
        {
            ServiceName = serviceName;
            DisplayName = serviceName;
            BinaryPath = binPath;
        }
    }
}
