using System;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [PublicAPI]
    public class ServiceDefinitionBuilder
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

        public ServiceDefinitionBuilder()
        {
        }

        public ServiceDefinitionBuilder(string serviceName)
        {
            ServiceName = serviceName;
        }
        
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
        }

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
        }

        public ServiceDefinition Build()
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
                ErrorSeverity = ErrorSeverity
            };
        }

        public ServiceDefinitionBuilder WithServiceName(string serviceName)
        {
            ServiceName = serviceName;
            return this;
        }

        public ServiceDefinitionBuilder WithDisplayName(string displayName)
        {
            DisplayName = displayName;
            return this;
        }

        public ServiceDefinitionBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public ServiceDefinitionBuilder WithBinaryPath(string binaryPath)
        {
            BinaryPath = binaryPath;
            return this;
        }

        public ServiceDefinitionBuilder WithCredentials(Win32ServiceCredentials credentials)
        {
            Credentials = credentials;
            return this;
        }

        public ServiceDefinitionBuilder WithFailureActions(ServiceFailureActions failureActions)
        {
            FailureActions = failureActions;
            return this;
        }

        public ServiceDefinitionBuilder WithFailureActionsOnNonCrashFailures(bool failureActionsOnNonCrashFailures)
        {
            FailureActionsOnNonCrashFailures = failureActionsOnNonCrashFailures;
            return this;
        }

        public ServiceDefinitionBuilder WithAutoStart(bool autoStart)
        {
            AutoStart = autoStart;
            return this;
        }

        public ServiceDefinitionBuilder WithErrorSeverity(ErrorSeverity errorSeverity)
        {
            ErrorSeverity = errorSeverity;
            return this;
        }
    }
}