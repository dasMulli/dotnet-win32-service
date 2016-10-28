using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Keep fields to preserve explicit struct layout for marshalling.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal struct ServiceDescriptionInfo
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        private string serviceDescription;

        public ServiceDescriptionInfo(string serviceDescription)
        {
            this.serviceDescription = serviceDescription;
        }

        public string ServiceDescription
        {
            get { return serviceDescription; }
            set { serviceDescription = value; }
        }
    }
}