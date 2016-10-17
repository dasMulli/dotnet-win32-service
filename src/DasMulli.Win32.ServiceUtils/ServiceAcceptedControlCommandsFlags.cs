using System;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [Flags]
    [PublicAPI]
    public enum ServiceAcceptedControlCommandsFlags : uint
    {
        None = 0,
        Stop = 0x00000001,
        PauseContinue = 0x00000002,
        Shutdown = 0x00000004,
        ParamChange = 0x00000008,
        NetBindChange = 0x00000010,
        PreShutdown = 0x00000100,
        HardwareProfileChange = 0x00000020,
        PowerEvent = 0x00000040,
        SessionChange = 0x00000080
    }
}