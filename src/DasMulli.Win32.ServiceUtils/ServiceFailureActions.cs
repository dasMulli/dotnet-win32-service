using System;
using System.Collections.Generic;
using System.Linq;

namespace DasMulli.Win32.ServiceUtils
{
    /// <inheritdoc />
    /// <summary>
    /// A managed class that holds data referring to a <see cref="T:DasMulli.Win32.ServiceUtils.ServiceFailureActionsInfo" /> class which has unmanaged resources
    /// </summary>
    public class ServiceFailureActions : IEquatable<ServiceFailureActions>
    {
        public TimeSpan ResetPeriod { get; }
        public string RebootMessage { get; }
        public string RestartCommand { get; }
        public IReadOnlyCollection<ScAction> Actions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActions"/> class.
        /// </summary>
        public ServiceFailureActions(TimeSpan resetPeriod, string rebootMessage, string restartCommand, IReadOnlyCollection<ScAction> actions)
        {
            ResetPeriod = resetPeriod;
            RebootMessage = rebootMessage;
            RestartCommand = restartCommand;
            Actions = actions;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ServiceFailureActions && Equals((ServiceFailureActions)obj);
        }


        public override int GetHashCode()
        {
            return HashCode
                .Of(this.ResetPeriod)
                .And(this.RebootMessage)
                .And(this.RestartCommand)
                .AndEach(this.Actions);
        }

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