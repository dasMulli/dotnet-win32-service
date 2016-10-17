using System;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ServiceTableEntry
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string serviceName;

        internal IntPtr serviceMainFunction;
    }
}