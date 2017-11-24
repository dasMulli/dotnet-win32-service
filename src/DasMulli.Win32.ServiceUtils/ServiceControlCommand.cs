using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Control command codes used issue control commands or
    /// notification events to services.
    /// </summary>
    [PublicAPI]
    public enum ServiceControlCommand : uint
    {
        /// <summary>
        /// Instructs a service to stop.
        /// </summary>
        Stop = 0x00000001,

        /// <summary>
        /// Instructs a service to pause.
        /// </summary>
        Pause = 0x00000002,

        /// <summary>
        /// Instructs a service to continue after being paused.
        /// </summary>
        Continue = 0x00000003,

        /// <summary>
        /// Instructs a service to report its current status to the service control manager.
        /// </summary>
        Interrogate = 0x00000004,

        /// <summary>
        /// Notifies a service of a shutdown.
        /// </summary>
        Shutdown = 0x00000005,

        /// <summary>
        /// Instructs a service to re-read its startup parameters.
        /// </summary>
        Paramchange = 0x00000006,

        /// <summary>
        /// Notifies a network service that there is a new
        /// component available for binding that it should bind to.
        /// </summary>
        NetBindAdd = 0x00000007,

        /// <summary>
        /// Notifies a network service that a component for binding
        /// has been removed and that it should unbind from it.
        /// </summary>
        NetBindRemoved = 0x00000008,

        /// <summary>
        /// Notifies a network service that a disabled binding
        /// has ben enabled and that it should add the new binding.
        /// </summary>
        NetBindEnable = 0x00000009,

        /// <summary>
        /// Notifies a network service that one of its bindings
        /// has ben disabled and that it should remove the binding.
        /// </summary>
        NetBindDisable = 0x0000000A,

        /// <summary>
        /// Notifies a service of device events.
        /// </summary>
        DeviceEvent = 0x0000000B,

        /// <summary>
        /// Notifies a service that the computer's hardware profile has changed.
        /// </summary>
        HardwareProfileChange = 0x0000000C,

        /// <summary>
        /// Notifies a service of system power events.
        /// </summary>
        PowerEvent = 0x0000000D,

        /// <summary>
        /// Notifies a service of session change events.
        /// </summary>
        SessionChange = 0x0000000E,

        /// <summary>
        /// Notifies a service that the system time has changed.
        /// </summary>
        TimeChange = 0x00000010,

        /// <summary>
        /// Notifies a service registered for a service trigger event that the event has occurred.
        /// </summary>
        TriggerEvent = 0x00000020,

        /// <summary>
        /// Notifies a service that the user has initiated a reboot.
        /// </summary>
        UserModeReboot = 0x00000040
    }
}