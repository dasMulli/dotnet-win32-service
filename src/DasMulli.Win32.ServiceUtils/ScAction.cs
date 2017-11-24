using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Service control actions used to specify what to do in case of service failures.
    /// </summary>
    /// <seealso cref="System.IEquatable{ScAction}" />
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Keep fields to preserve explicit struct layout for marshalling.")]
    [PublicAPI]
    public struct ScAction:IEquatable<ScAction>
    {
        private ScActionType _Type;
        private uint _Delay;

        /// <summary>
        /// Gets or sets the type of service control action.
        /// </summary>
        /// <value>
        /// The type of service control action.
        /// </value>
        public ScActionType Type
        {
            get => _Type;
            set => _Type = value;
        }

        /// <summary>
        /// Gets or sets the amount of time the action is to be delayed when a failure occurs.
        /// </summary>
        /// <value>
        /// The amount of time the action is to be delayed when a failure occurs.
        /// </value>
        public TimeSpan Delay
        {
            get => TimeSpan.FromMilliseconds(_Delay);
            set => _Delay = (uint)Math.Round(value.TotalMilliseconds);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ScAction other)
        {
            return _Type == other._Type && _Delay == other._Delay;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ScAction && Equals((ScAction)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode
                .Of(this.Delay)
                .And(this.Type);
        }
    }
}