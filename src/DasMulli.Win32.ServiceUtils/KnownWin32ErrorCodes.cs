using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class KnownWin32ErrorCoes {
        internal const int ERROR_SERVICE_ALREADY_RUNNING = 1056;
        internal const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;
    }
}