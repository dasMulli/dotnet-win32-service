using System;
using System.ComponentModel;

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

        public void CreateService(string serviceName, string displayName, string description, string binaryPath, Win32ServiceCredentials credentials,
            bool autoStart = false, bool startImmediately = false, ErrorSeverity errorSeverity = ErrorSeverity.Normal)
        {
            CreateService(
                new ServiceDefinitionBuilder(serviceName)
                    .WithDisplayName(displayName)
                    .WithDescription(description)
                    .WithBinaryPath(binaryPath)
                    .WithCredentials(credentials)
                    .WithAutoStart(autoStart)
                    .WithErrorSeverity(errorSeverity)
                    .Build(),
                startImmediately
            );
        }

        public void CreateService(ServiceDefinition serviceDefinition, bool startImmediately = false)
        {
            if (string.IsNullOrEmpty(serviceDefinition.BinaryPath))
            {
                throw new ArgumentException($"Invalid service definition. {nameof(ServiceDefinition.BinaryPath)} must not be null or empty.", nameof(serviceDefinition));
            }
            if (string.IsNullOrEmpty(serviceDefinition.ServiceName))
            {
                throw new ArgumentException($"Invalid service definition. {nameof(ServiceDefinition.ServiceName)} must not be null or empty.", nameof(serviceDefinition));
            }

            try
            {
                using (var mgr = ServiceControlManager.Connect(nativeInterop, machineName, databaseName, ServiceControlManagerAccessRights.All))
                {
                    DoCreateService(mgr, serviceDefinition, startImmediately);
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.", dllException);
            }
        }

        private void DoCreateService(ServiceControlManager serviceControlManager, ServiceDefinition serviceDefinition, bool startImmediately)
        {
            using (var svc = serviceControlManager.CreateService(serviceDefinition.ServiceName, serviceDefinition.DisplayName, serviceDefinition.BinaryPath, ServiceType.Win32OwnProcess,
                    serviceDefinition.AutoStart ? ServiceStartType.AutoStart : ServiceStartType.StartOnDemand, serviceDefinition.ErrorSeverity, serviceDefinition.Credentials))
            {
                var description = serviceDefinition.Description;
                if (!string.IsNullOrEmpty(description))
                {
                    svc.SetDescription(description);
                }

                var serviceFailureActions = serviceDefinition.FailureActions;
                if (serviceFailureActions != null)
                {
                    svc.SetFailureActions(serviceFailureActions);
                    svc.SetFailureActionFlag(serviceDefinition.FailureActionsOnNonCrashFailures);
                }

                if (startImmediately)
                {
                    svc.Start();
                }
            }
        }
        
        public void CreateOrUpdateService(ServiceDefinition serviceDefinition, bool startImmediately = false)
        {
            if (string.IsNullOrEmpty(serviceDefinition.BinaryPath))
            {
                throw new ArgumentException($"Invalid service definition. {nameof(ServiceDefinition.BinaryPath)} must not be null or empty.", nameof(serviceDefinition));
            }
            if (string.IsNullOrEmpty(serviceDefinition.ServiceName))
            {
                throw new ArgumentException($"Invalid service definition. {nameof(ServiceDefinition.ServiceName)} must not be null or empty.", nameof(serviceDefinition));
            }

            try
            {
                using (var mgr = ServiceControlManager.Connect(nativeInterop, machineName, databaseName, ServiceControlManagerAccessRights.All))
                {
                    if (mgr.TryOpenService(serviceDefinition.ServiceName, ServiceControlAccessRights.All, out var existingService, out var errorException)) {
                        using(existingService)
                        {
                            DoUpdateService(existingService, serviceDefinition, startImmediately);
                        }
                    } else {
                        if (errorException.NativeErrorCode == KnownWin32ErrorCoes.ERROR_SERVICE_DOES_NOT_EXIST) {
                            DoCreateService(mgr, serviceDefinition, startImmediately);
                        } else {
                            throw errorException;
                        }
                    }
                }
            }
            catch (DllNotFoundException dllException)
            {
                throw new PlatformNotSupportedException(nameof(Win32ServiceHost) + " is only supported on Windows with service management API set.", dllException);
            }
        }

        private static void DoUpdateService(ServiceHandle existingService, ServiceDefinition serviceDefinition, bool startIfNotRunning)
        {
            existingService.ChangeConfig(serviceDefinition.DisplayName, serviceDefinition.BinaryPath, ServiceType.Win32OwnProcess,
                serviceDefinition.AutoStart ? ServiceStartType.AutoStart : ServiceStartType.StartOnDemand, serviceDefinition.ErrorSeverity,
                serviceDefinition.Credentials);
            existingService.SetDescription(serviceDefinition.Description);
            existingService.SetFailureActions(serviceDefinition.FailureActions);
            existingService.SetFailureActionFlag(serviceDefinition.FailureActionsOnNonCrashFailures);
            if (startIfNotRunning)
            {
                existingService.Start(throwIfAlreadyRunning: false);
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
