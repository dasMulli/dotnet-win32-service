using System;
using System.Collections.Generic;

namespace DasMulli.Win32.ServiceUtils
{
    public class ServiceFailureActions
    {
        public TimeSpan ResetPeriod { get; set; }
        public string RebootMessage { get; set; }
        public string RestartCommand { get; set; }
        public IReadOnlyCollection<ScAction> Actions { get; set; }

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
    }
}