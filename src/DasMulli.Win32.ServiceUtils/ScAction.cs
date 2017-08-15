using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Keep fields to preserve explicit struct layout for marshalling.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    public struct ScAction:IEquatable<ScAction>
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

        public bool Equals(ScAction other)
        {
            return _Type == other._Type && _Delay == other._Delay;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ScAction && Equals((ScAction)obj);
        }

        public override int GetHashCode()
        {
            return HashCode
                .Of(this.Delay)
                .And(this.Type);
        }
    }
}