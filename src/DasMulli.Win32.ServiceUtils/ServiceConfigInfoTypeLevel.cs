using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal enum ServiceConfigInfoTypeLevel : uint
    {
        ServiceDescription = 1,
        FailureActions = 2,
        DelayedAutoStartInfo = 3,
        FailureActionsFlag = 4,
        ServiceSidInfo = 5,
        RequiredPrivilegesInfo = 6,
        PreShutdownInfo = 7,
        TriggerInfo = 8,
        PreferredNode = 9,
        LaunchProtected = 12
    }
}