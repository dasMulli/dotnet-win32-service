using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Contains settings for a windows service registration.
    /// </summary>
    [PublicAPI]
    public class ServiceDefinition
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the service.
        /// </summary>
        /// <value>
        /// The display name of the service.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the service description.
        /// </summary>
        /// <value>
        /// The service description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the binary path of the service.
        /// This includes the path to the executable as well as the
        /// arguments to be passed to it.
        /// </summary>
        /// <value> 
        /// The binary path of the service.
        /// This includes the path to the executable as well as the
        /// arguments to be passed to it.
        /// </value>
        public string BinaryPath { get; set; }

        /// <summary>
        /// Gets or sets the credentials for the account the service shall run as.
        /// </summary>
        /// <value>
        /// The credentials for the account the service shall run as.
        /// </value>
        public Win32ServiceCredentials Credentials { get; set; } = Win32ServiceCredentials.LocalSystem;

        /// <summary>
        /// Gets or sets the failure actions of the service.
        /// </summary>
        /// <value>
        /// The failure actions of the service.
        /// </value>
        public ServiceFailureActions FailureActions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the failure actions will be triggered
        /// even if the service reports stopped but with a non-zero exit code.
        /// If false, the failure actions will only be triggered if the service terminates
        /// without reporting the stopped state (=> considered a crash).
        /// </summary>
        /// <value>
        /// When <c>true</c>, the configured failure actions will be triggered
        /// even if the service reports stopped but with a non-zero exit code.
        /// If <c>fasle</c>, the failure actions will only be triggered if the service terminates
        /// without reporting the stopped state (=> considered a crash).
        /// </value>
        public bool FailureActionsOnNonCrashFailures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service shall be started automatically during system startup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the service shall be started automatically during system startup; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; } = true;

        /// <summary>
        /// Gets or sets the error severity of service failures.
        /// </summary>
        /// <value>
        /// The error severity of service failures.
        /// </value>
        public ErrorSeverity ErrorSeverity { get; set; } = ErrorSeverity.Normal;

        /// <summary>
        /// Gets or sets a value indicating whether the service shall started delayed when started
        /// automatically on startup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the service shall started delayed when started
        /// automatically on startup; otherwise, <c>false</c>.
        /// </value>
        public bool DelayedAutoStart { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinition"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="binPath">
        /// The binary path of the service.
        /// This includes the path to the executable as well as the
        /// arguments to be passed to it.
        /// </param>
        public ServiceDefinition(string serviceName, string binPath)
        {
            ServiceName = serviceName;
            DisplayName = serviceName;
            BinaryPath = binPath;
        }
    }
}
