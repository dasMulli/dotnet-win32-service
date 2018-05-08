using System;
using System.Collections.Generic;

namespace DasMulli.Win32.ServiceUtils
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a set of configurations that specify which actions to take if a service fails.
    /// 
    /// A managed class that holds data referring to a <see cref="T:DasMulli.Win32.ServiceUtils.ServiceFailureActionsInfo" /> class which has unmanaged resources
    /// </summary>
    public class ServiceFailureActions : IEquatable<ServiceFailureActions>
    {
        /// <summary>
        /// Gets the reset period in seconds after which previous failures are cleared.
        /// For example: When a service fails two times and then doesn't fail for this amount of time, then an
        /// additional failure is considered a first failure and not a third.
        /// </summary>
        /// <value>
        /// The reset period in seconds after which previous failures are cleared.
        /// For example: When a service fails two times and then doesn't fail for this amount of time, then an
        /// additional failure is considered a first failure and not a third.
        /// </value>
        public TimeSpan ResetPeriod { get; }

        /// <summary>
        /// Gets the reboot message used in case a reboot failure action is configured.
        /// </summary>
        /// <value>
        /// The reboot message used in case a reboot failure action is configured.
        /// </value>
        public string RebootMessage { get; }

        /// <summary>
        /// Gets the command run in case a "run command" failure action is configured.
        /// </summary>
        /// <value>
        /// The command run in case a "run command" failure action is configured.
        /// </value>
        public string RestartCommand { get; }

        /// <summary>
        /// Gets the collections of configured failure actions for each successive time the service failes.
        /// </summary>
        /// <value>
        /// The collections of configured failure actions for each successive time the service failes.
        /// </value>
        public IReadOnlyCollection<ScAction> Actions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActions" /> class.
        /// </summary>
        /// <param name="resetPeriod">The reset period in seconds after which previous failures are cleared.</param>
        /// <param name="rebootMessage">The reboot message used in case a reboot failure action is contaiend in <paramref name="actions"/>.</param>
        /// <param name="restartCommand">The command run in case a "run command" failure action is contained in <paramref name="actions"/>.</param>
        /// <param name="actions">The failure actions.</param>
        public ServiceFailureActions(TimeSpan resetPeriod, string rebootMessage, string restartCommand, IReadOnlyCollection<ScAction> actions)
        {
            ResetPeriod = resetPeriod;
            RebootMessage = rebootMessage;
            RestartCommand = restartCommand;
            Actions = actions;
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
            return obj is ServiceFailureActions && Equals((ServiceFailureActions)obj);
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
                .Of(this.ResetPeriod)
                .And(this.RebootMessage)
                .And(this.RestartCommand)
                .AndEach(this.Actions);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ServiceFailureActions other)
        {
            if (other == null)
            {
                return false;
            }
            return this.GetHashCode() == other.GetHashCode();
        }
    }
}