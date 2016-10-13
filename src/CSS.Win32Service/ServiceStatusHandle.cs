using System;
using System.Runtime.InteropServices;

namespace CSS.ServiceHost
{
    internal class ServiceStatusHandle : SafeHandle
    {
        internal ServiceStatusHandle() : base(IntPtr.Zero, ownsHandle: true)
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
    }
}