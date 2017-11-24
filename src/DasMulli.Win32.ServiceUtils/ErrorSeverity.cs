using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Specifies the severity of the error if the service fails to start during boot 
    /// </summary>
    [PublicAPI]
    public enum ErrorSeverity : uint
    {
        /// <summary>
        /// SC.exe help:
        /// The error is logged and startup continues. No notification is given to the user beyond recording the error in the Event Log.
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// SC.exe help:
        /// The error is logged and a message box is displayed informing the user that a service has failed to start. Startup will continue. This is the default setting.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// SC.exe help:
        /// The error is logged (if possible). The computer attempts to restart with the last-known-good configuration. This could result in the computer being able to restart, but the service may still be unable to run.
        /// </summary>
        Severe = 2,
        /// <summary>
        /// SC.exe help:
        /// The error is logged (if possible). The computer attempts to restart with the last-known-good configuration. If the last-known-good configuration fails, startup also fails, and the boot process halts with a Stop error.
        /// </summary>
        Crititcal = 3
    }
}