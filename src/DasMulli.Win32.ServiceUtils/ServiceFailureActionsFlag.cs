using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ServiceFailureActionsFlag
    {
        private bool _fFailureActionsOnNonCrashFailures;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActionsFlag"/> struct.
        /// </summary>
        internal ServiceFailureActionsFlag(bool enabled)
        {
            _fFailureActionsOnNonCrashFailures = enabled;
        }
    }
}