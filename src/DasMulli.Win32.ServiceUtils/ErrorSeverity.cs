using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [PublicAPI]
    public enum ErrorSeverity : uint
    {
        Ignore = 0,
        Normal = 1,
        Severe = 2,
        Crititcal = 3
    }
}