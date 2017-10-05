namespace DasMulli.Win32.ServiceUtils.Tests
{
    internal static class ServiceDefinitionBuilderExtensions
    {
        public static ServiceDefinition BuildNonValidating(this ServiceDefinitionBuilder builder)
        {
            return new ServiceDefinition(builder.ServiceName, builder.BinaryPath)
            {
                DisplayName = builder.DisplayName,
                Description = builder.Description,
                Credentials = builder.Credentials,
                FailureActions = builder.FailureActions,
                FailureActionsOnNonCrashFailures = builder.FailureActionsOnNonCrashFailures,
                AutoStart = builder.AutoStart,
                ErrorSeverity = builder.ErrorSeverity
            };
        }
    }
}