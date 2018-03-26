using System;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Values used to indicate which service control commands are accepted by a service.
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum ServiceAcceptedControlCommandsFlags : uint
    {
        /// <summary>
        /// No command is accepted. Usually used during StartPending/StopPending/PausePending states.
        /// </summary>
        None = 0,

        /// <summary>
        /// The service can be stopped
        /// </summary>
        Stop = 0x00000001,

        /// <summary>
        /// The service can be paused when running or continued when currently paused.
        /// </summary>
        PauseContinue = 0x00000002,

        /// <summary>
        /// The service can be stopped or paused when running or continued when currently paused.
        /// </summary>
        PauseContinueStop = PauseContinue | Stop,

        /// <summary>
        /// The shutdown command is accepted which notfies the service about a system shutdown.
        /// This event can only be sent by the system.
        /// </summary>
        Shutdown = 0x00000004,

        /// <summary>
        /// Indicates that the service is expected to re-read its startup parameters without needing to be restarted.
        /// </summary>
        ParamChange = 0x00000008,

        /// <summary>
        /// Tndicates that the service is a network service that can re-read its binding parameters without needing to be restarted.
        /// </summary>
        NetBindChange = 0x00000010,

        /// <summary>
        /// Indicates that the service can perform pre-sutdown tasks.
        /// This event can only be sent by the system.
        /// </summary>
        PreShutdown = 0x00000100,

        /// <summary>
        /// The service can react to system hardware changes.
        /// This event can only be sent by the system.
        /// </summary>
        HardwareProfileChange = 0x00000020,

        /// <summary>
        /// The service can react to power status cahnges.
        /// This event can only be sent by the system.
        /// </summary>
        PowerEvent = 0x00000040,

        /// <summary>
        /// The service can react to system session status changes.
        /// This event can only be sent by the system.
        /// </summary>
        SessionChange = 0x00000080,

        /// <summary>
        /// The service can react to system time changes.
        /// Only supported on Windows 7 / Windows Server 2008 R2 and higher
        /// This event can only be sent by the system.
        /// </summary>
        TimeChange = 0x00000200,

        /// <summary>
        /// The service can be notified when an event for which the service has explicitly registered occurs.
        /// This event can only be sent by the system.
        /// </summary>
        TriggerEvent = 0x00000400,

        /// <summary>
        /// The service can react when a user initiates a reboot.
        /// This event can only be sent by the system.
        /// </summary>
        UserModeReboot = 0x00000800
    }
}