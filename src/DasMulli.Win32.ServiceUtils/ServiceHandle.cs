using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Subclassed by test proxy")]
    internal class ServiceHandle : SafeHandle
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Exposed for testing via InternalsVisibleTo.")]
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

        public virtual void Start()
        {
            if (!NativeInterop.StartServiceW(this, 0, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void Delete()
        {
            if (!NativeInterop.DeleteService(this))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void SetDescription(string description)
        {
            var descriptionInfo = new ServiceDescriptionInfo(description ?? string.Empty);
            var lpDescriptionInfo = Marshal.AllocHGlobal(Marshal.SizeOf<ServiceDescriptionInfo>());
            try
            {
                Marshal.StructureToPtr(descriptionInfo, lpDescriptionInfo, fDeleteOld: false);
                if (!NativeInterop.ChangeServiceConfig2W(this, ServiceConfigInfoTypeLevel.ServiceDescription, lpDescriptionInfo))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(lpDescriptionInfo);
            }
        }
    }
}