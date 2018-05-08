using System;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Allows managing registered windows services on a machine
    /// </summary>
    public sealed class Win32ServiceManager
    {
        private readonly string machineName;
        private readonly string databaseName;
        private readonly INativeInterop nativeInterop;

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32ServiceManager"/> class that
        /// can manage windows services on a specified machine and configuration databse.
        /// </summary>
        /// <param name="machineName">Name of the machine to mamage.</param>
        /// <param name="databaseName">Name of the database to maange.</param>
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

        /// <summary>
        /// Creates a new windows service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="displayName">The display name of the service.</param>
        /// <param name="description">The description of the service.</param>
        /// <param name="binaryPath">The path to the binary to use as windows service including arguments.</param>
        /// <param name="credentials">The credentials used to run the servic ewith.</param>
        /// <param name="autoStart">if set to <c>true</c> the service will start automatically during boot.</param>
        /// <param name="startImmediately">if set to <c>true</c> the service will be started immediatly after registering.</param>
        /// <param name="errorSeverity">The error severity of the service.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <paramref name="binaryPath"/> is null or empty
        /// or
        /// <paramref name="serviceName"/> is null or empty
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when run on a non-windows platform.</exception>
        [Obsolete("Use the CreateService() overload taking a ServiceDefinition argument instead. This method only exists for backwards compatibility.")]
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

        /// <summary>
        /// Creates the service.
        /// </summary>
        /// <param name="serviceDefinition">The service definition.</param>
        /// <param name="startImmediately">if set to <c>true</c> the service will be started immediatly after registering.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// BinaryPath of <paramref name="serviceDefinition"/> is null or empty
        /// or
        /// ServiceName of <paramref name="serviceDefinition"/> is null or empty
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when run on a non-windows platform.</exception>
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
                
                if (serviceDefinition.AutoStart && serviceDefinition.DelayedAutoStart)
                {
                    svc.SetDelayedAutoStartFlag(true);
                }

                if (startImmediately)
                {
                    svc.Start();
                }
            }
        }

        /// <summary>
        /// Creates the or update a windows service.
        /// Note that the service is not restarted due to changes in its configuration
        /// </summary>
        /// <param name="serviceDefinition">The service definition.</param>
        /// <param name="startImmediately">if set to <c>true</c> the service will be started immediatly after updating. Has no effect if the service is already running.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// BinaryPath of <paramref name="serviceDefinition"/> is null or empty
        /// or
        /// ServiceName of <paramref name="serviceDefinition"/> is null or empty
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when run on a non-windows platform.</exception>
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
            existingService.SetDelayedAutoStartFlag(serviceDefinition.AutoStart && serviceDefinition.DelayedAutoStart);
            if (startIfNotRunning)
            {
                existingService.Start(throwIfAlreadyRunning: false);
            }
        }

        /// <summary>
        /// Deletes a windows service.
        /// </summary>
        /// <param name="serviceName">Name of the service to delete.</param>
        /// <exception cref="ArgumentException">Trown when <paramref name="serviceName"/> is null or empty.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when run on a non-windows platform.</exception>
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
