using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CSS.ServiceHost
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    public struct ServiceStatus
    {
        private ServiceType serviceType;
        private ServiceState state;
        private ServiceAcceptedControlCommandsFlags acceptedControlCommands;
        private uint win32ExitCode;
        private uint serviceSpecificExitCode;
        private uint checkPoint;
        private uint waitHint;

        public ServiceType ServiceType
        {
            get { return serviceType; }
            set { serviceType = value; }
        }

        public ServiceState State
        {
            get { return state; }
            set { state = value; }
        }

        public ServiceAcceptedControlCommandsFlags AcceptedControlCommands
        {
            get { return acceptedControlCommands; }
            set { acceptedControlCommands = value; }
        }

        public uint Win32ExitCode
        {
            get { return win32ExitCode; }
            set { win32ExitCode = value; }
        }

        public uint ServiceSpecificExitCode
        {
            get { return serviceSpecificExitCode; }
            set { serviceSpecificExitCode = value; }
        }

        public uint CheckPoint
        {
            get { return checkPoint; }
            set { checkPoint = value; }
        }

        public uint WaitHint
        {
            get { return waitHint; }
            set { waitHint = value; }
        }

        public ServiceStatus(ServiceType serviceType, ServiceState state, ServiceAcceptedControlCommandsFlags acceptedControlCommands, uint win32ExitCode, uint serviceSpecificExitCode, uint checkPoint, uint waitHint)
        {
            this.serviceType = serviceType;
            this.state = state;
            this.acceptedControlCommands = acceptedControlCommands;
            this.win32ExitCode = win32ExitCode;
            this.serviceSpecificExitCode = serviceSpecificExitCode;
            this.checkPoint = checkPoint;
            this.waitHint = waitHint;
        }
    }
}