using System;
using JetBrains.Annotations;

namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Represents credentials for accounts to run Windows services with.
    /// </summary>
    /// <seealso cref="System.IEquatable{T}" />
    [PublicAPI]
    public struct Win32ServiceCredentials : IEquatable<Win32ServiceCredentials>
    {
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; }

        /// <summary>
        /// Creates a new <see cref="Win32ServiceCredentials"/> instance to represent an account under which to run Windows services.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="password">The password.</param>
        public Win32ServiceCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// The Local System account. The service will have full access to the system and machine network credentials.
        /// Not recommended to use in production environments.
        /// </summary>
        public static Win32ServiceCredentials LocalSystem = new Win32ServiceCredentials(userName: null, password: null);

        /// <summary>
        /// The Local Service account. The service will have minimum access to the system and anonymous network credentials.
        /// Recommended for use in logic-only applications.
        /// Consider using a custom account instead for granular control over file system permissions.
        /// </summary>
        public static Win32ServiceCredentials LocalService = new Win32ServiceCredentials(@"NT AUTHORITY\LocalService", password: null);

        /// <summary>
        /// The Network Service account. The service will have minimum access to the system and machine network credentials.
        /// Recommended for use in logic-only applications that need to authenticate to networks using machine credentials.
        /// Consider using a custom account instead for granular control over file system permissions and network authorization control.
        /// </summary>
        public static Win32ServiceCredentials NetworkService = new Win32ServiceCredentials(@"NT AUTHORITY\NetworkService", password: null);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Win32ServiceCredentials other)
        {
            return string.Equals(UserName, other.UserName) && string.Equals(Password, other.Password);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(objA: null, objB: obj))
            {
                return false;
            }
            return obj is Win32ServiceCredentials && Equals((Win32ServiceCredentials) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserName?.GetHashCode() ?? 0)*397) ^ (Password?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Implements the operator <c>==</c>.
        /// </summary>
        public static bool operator ==(Win32ServiceCredentials left, Win32ServiceCredentials right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator <c>!=</c>.
        /// </summary>
        public static bool operator !=(Win32ServiceCredentials left, Win32ServiceCredentials right)
        {
            return !left.Equals(right);
        }
    }
}