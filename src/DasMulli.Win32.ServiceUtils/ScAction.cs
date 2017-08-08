using System;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ScAction
    {
        private ScActionType _Type;
        private uint _Delay;
        
        public ScActionType Type
        {
            get => _Type;
            set => _Type = value;
        }

        public TimeSpan Delay
        {
            get => TimeSpan.FromMilliseconds(_Delay);
        
            set => _Delay = (uint)Math.Round(value.TotalMilliseconds);
        }
    }
}