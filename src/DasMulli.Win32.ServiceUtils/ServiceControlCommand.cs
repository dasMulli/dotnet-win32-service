using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [PublicAPI]
    public enum ServiceControlCommand : uint
    {
        Stop = 0x00000001,
        Pause = 0x00000002,
        Continue = 0x00000003,
        Interrogate = 0x00000004,
        Shutdown = 0x00000005,
        Paramchange = 0x00000006,
        NetBindAdd = 0x00000007,
        NetBindRemoved = 0x00000008,
        NetBindEnable = 0x00000009,
        NetBindDisable = 0x0000000A,
        DeviceEvent = 0x0000000B,
        HardwareProfileChange = 0x0000000C,
        PowerEvent = 0x0000000D,
        SessionChange = 0x0000000E
    }
}