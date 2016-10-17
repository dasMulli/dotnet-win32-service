using System;
using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [Flags]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal enum ServiceTypeFilter : uint
    {
        FileSystemDriver = 0x00000002,
        KernelDriver = 0x00000001,
        Win32OwnProcess = 0x00000010,
        Win32ShareProcess = 0x00000020,
        InteractiveProcess = 0x00000100,
        All = FileSystemDriver | KernelDriver | Win32OwnProcess | Win32ShareProcess | InteractiveProcess,
        Win32 = Win32OwnProcess | Win32ShareProcess,
        Driver = FileSystemDriver | KernelDriver
    }
}