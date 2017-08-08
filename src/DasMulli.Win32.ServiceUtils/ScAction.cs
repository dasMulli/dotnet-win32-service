using System;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ScAction
    {
        private ScActionType _Type;
        private uint _Delay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScAction"/> class.
        /// </summary>
        public ScAction(ScActionType type, TimeSpan delay)
        {
            _Type = type;
            _Delay = (uint) Math.Round(delay.TotalMilliseconds);
        }

        public ScActionType Type
        {
            get => _Type;
            set => _Type = value;
        }

        public uint Delay
        {
            get => _Delay;
            set => _Delay = value;
        }
    }
}