using JetBrains.Annotations;

namespace CSS.Win32Service
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