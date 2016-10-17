using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal enum ServiceStartType : uint
    {
        StartOnBoot = 0,
        StartOnSystemStart = 1,
        AutoStart = 2,
        StartOnDemand = 3,
        Disabled = 4
    }
}