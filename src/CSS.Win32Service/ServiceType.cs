using JetBrains.Annotations;

namespace CSS.Win32Service
{
    // TODO: decide if this shall be exposed
    [PublicAPI]
    public enum ServiceType : uint
    {
        FileSystemDriver = 0x00000002,
        KernelDriver = 0x00000001,
        Win32OwnProcess = 0x00000010,
        Win32ShareProcess = 0x00000020,
        InteractiveProcess = 0x00000100
    }
}