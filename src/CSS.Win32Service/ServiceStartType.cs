using JetBrains.Annotations;

namespace CSS.Win32Service
{
    // TODO: really expose? only a bool flag for autostart is currently exposed through Win32ServiceManager
    [PublicAPI]
    public enum ServiceStartType : uint
    {
        StartOnBoot = 0,
        StartOnSystemStart = 1,
        AutoStart = 2,
        StartOnDemand = 3,
        Disabled = 4
    }
}