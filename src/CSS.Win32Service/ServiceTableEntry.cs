using System;
using System.Runtime.InteropServices;

namespace CSS.Win32Service
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ServiceTableEntry
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string serviceName;

        internal IntPtr serviceMainFunction;
    }
}