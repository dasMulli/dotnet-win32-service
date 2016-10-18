using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    internal class ServiceHandle : SafeHandle
    {
        internal INativeInterop NativeInterop { get; set; } = Win32Interop.Wrapper;

        internal ServiceHandle() : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeInterop.CloseServiceHandle(handle);
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
            if (!NativeInterop.StartServiceW(this, 0, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void Delete()
        {
            if (!NativeInterop.DeleteService(this))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}