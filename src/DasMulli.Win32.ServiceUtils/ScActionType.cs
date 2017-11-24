using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Action types used for failure actions.
    /// Used in the <see cref="ScAction"/> type.
    /// </summary>
    [PublicAPI]
    public enum ScActionType
    {
        /// <summary>
        /// No action
        /// </summary>
        ScActionNone = 0,

        /// <summary>
        /// Restart service
        /// </summary>
        ScActionRestart = 1,

        /// <summary>
        /// Reboot the computer (meant to be used for drivers and not in managed services)
        /// </summary>
        ScActionReboot = 2,

        /// <summary>
        /// Run a command
        /// </summary>
        ScActionRunCommand = 3,
    }
}