using System;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Builder class used to construct instances of <see cref="ServiceDefinition"/> instances.
    /// </summary>
    [PublicAPI]
    public class ServiceDefinitionBuilder
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
        /// Initializes a new instance of the <see cref="ServiceDefinitionBuilder"/> class.
        /// </summary>
        public ServiceDefinitionBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionBuilder"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        public ServiceDefinitionBuilder(string serviceName)
        {
            ServiceName = serviceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionBuilder"/> class
        /// using the values of an existing <see cref="ServiceDefinition"/> instance.
        /// </summary>
        /// <param name="definition">The service definition to copy values from.</param>
        public ServiceDefinitionBuilder(ServiceDefinition definition)
        {
            ServiceName = definition.ServiceName;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            BinaryPath = definition.BinaryPath;
            Credentials = definition.Credentials;
            FailureActions = definition.FailureActions;
            FailureActionsOnNonCrashFailures = definition.FailureActionsOnNonCrashFailures;
            AutoStart = definition.AutoStart;
            ErrorSeverity = definition.ErrorSeverity;
            DelayedAutoStart = definition.DelayedAutoStart;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionBuilder"/> class
        /// using the values of an existing <see cref="ServiceDefinitionBuilder"/> instance.
        /// </summary>
        /// <param name="definitionBuilder">The existing builder to copy values from.</param>
        public ServiceDefinitionBuilder(ServiceDefinitionBuilder definitionBuilder)
        {
            ServiceName = definitionBuilder.ServiceName;
            DisplayName = definitionBuilder.DisplayName;
            Description = definitionBuilder.Description;
            BinaryPath = definitionBuilder.BinaryPath;
            Credentials = definitionBuilder.Credentials;
            FailureActions = definitionBuilder.FailureActions;
            FailureActionsOnNonCrashFailures = definitionBuilder.FailureActionsOnNonCrashFailures;
            AutoStart = definitionBuilder.AutoStart;
            ErrorSeverity = definitionBuilder.ErrorSeverity;
            DelayedAutoStart = definitionBuilder.DelayedAutoStart;
        }

        /// <summary>
        /// Builds a new instance of <see cref="ServiceDefinition"/> using the configured values.
        /// </summary>
        /// <returns>New instance of <see cref="ServiceDefinitionBuilder"/></returns>
        /// <exception cref="InvalidOperationException">
        /// Thown when <see cref="ServiceName"/> or <see cref="BinaryPath"/> are null or empty.
        /// </exception>
        public virtual ServiceDefinition Build()
        {
            if (string.IsNullOrEmpty(ServiceName))
            {
                throw new InvalidOperationException($"Cannot create a ServiceDefinition when {nameof(ServiceName)} is not set to a non-empty string");
            }

            if (string.IsNullOrEmpty(BinaryPath))
            {
                throw new InvalidOperationException($"Cannot create a ServiceDefinition when {nameof(BinaryPath)} is not set to a non-empty string");
            }

            return new ServiceDefinition(ServiceName, BinaryPath)
            {
                DisplayName = DisplayName,
                Description = Description,
                Credentials = Credentials,
                FailureActions = FailureActions,
                FailureActionsOnNonCrashFailures = FailureActionsOnNonCrashFailures,
                AutoStart = AutoStart,
                ErrorSeverity = ErrorSeverity,
                DelayedAutoStart = DelayedAutoStart
            };
        }

        /// <summary>
        /// Changes the name of the service.
        /// </summary>
        /// <param name="serviceName">New name of the service.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithServiceName(string serviceName)
        {
            ServiceName = serviceName;
            return this;
        }

        /// <summary>
        /// Changes the display name of the service.
        /// </summary>
        /// <param name="displayName">The new display name of the service.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithDisplayName(string displayName)
        {
            DisplayName = displayName;
            return this;
        }

        /// <summary>
        /// Changes the service description.
        /// </summary>
        /// <param name="description">The new service description.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Changes the binary path of the service.
        /// This includes the path to the executable as well as the
        /// arguments to be passed to it.
        /// </summary>
        /// <param name="binaryPath">
        /// The new binary path of the service.
        /// This includes the path to the executable as well as the
        /// arguments to be passed to it.
        /// </param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithBinaryPath(string binaryPath)
        {
            BinaryPath = binaryPath;
            return this;
        }

        /// <summary>
        /// Changes the credentials for the account the service shall run as.
        /// </summary>
        /// <param name="credentials">The new credentials for the account the service shall run as.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithCredentials(Win32ServiceCredentials credentials)
        {
            Credentials = credentials;
            return this;
        }

        /// <summary>
        /// Changes the failure actions of the service.
        /// </summary>
        /// <param name="failureActions">The new failure actions of the service.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithFailureActions(ServiceFailureActions failureActions)
        {
            FailureActions = failureActions;
            return this;
        }

        /// <summary>
        /// Changes the flag to trigger failure actions on non crash failures.
        /// </summary>
        /// <param name="failureActionsOnNonCrashFailures">
        /// When <c>true</c>, the configured failure actions will be triggered
        /// even if the service reports stopped but with a non-zero exit code.
        /// If <c>fasle</c>, the failure actions will only be triggered if the service terminates
        /// without reporting the stopped state (=> considered a crash).
        /// </param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithFailureActionsOnNonCrashFailures(bool failureActionsOnNonCrashFailures)
        {
            FailureActionsOnNonCrashFailures = failureActionsOnNonCrashFailures;
            return this;
        }

        /// <summary>
        /// Chagnges the flag indicating whether the service shall be started automatically during system startup.
        /// </summary>
        /// <param name="autoStart">
        ///   <c>true</c> if the service shall be started automatically during system startup; otherwise, <c>false</c>.
        /// </param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithAutoStart(bool autoStart)
        {
            AutoStart = autoStart;
            return this;
        }

        /// <summary>
        /// Changes the error severity of service failures.
        /// </summary>
        /// <param name="errorSeverity">The new error severity of service failures.</param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithErrorSeverity(ErrorSeverity errorSeverity)
        {
            ErrorSeverity = errorSeverity;
            return this;
        }

        /// <summary>
        /// Changes the delayed automatic start flag indicating whether the service shall started delayed when started
        /// automatically on startup.
        /// </summary>
        /// <param name="delayedAutoStart">
        ///   <c>true</c> if the service shall started delayed when started
        /// automatically on startup; otherwise, <c>false</c>.
        /// </param>
        /// <returns>Returns the current instance</returns>
        public ServiceDefinitionBuilder WithDelayedAutoStart(bool delayedAutoStart)
        {
            DelayedAutoStart = delayedAutoStart;
            return this;
        }
    }
}