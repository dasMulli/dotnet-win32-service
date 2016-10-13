using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CSS.ServiceHost
{
    internal class ServiceHandle : SafeHandle
    {
        internal ServiceHandle() : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.CloseServiceHandle(handle);
        }

        public override bool IsInvalid
        {
            [System.Security.SecurityCritical]
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        public void Start()
        {
            if (!Interop.StartServiceW(this, 0, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void Delete()
        {
            if (!Interop.DeleteService(this))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}