using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    internal class ServiceStatusHandle : SafeHandle
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Exposed for testing via InternalsVisibleTo.")]
        internal INativeInterop NativeInterop { get; set; } = Win32Interop.Wrapper;

        internal ServiceStatusHandle() : base(IntPtr.Zero, ownsHandle: true)
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
    }
}