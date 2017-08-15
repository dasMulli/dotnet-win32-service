using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Keep fields to preserve explicit struct layout for marshalling.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal struct ServiceFailureActionsInfo
    {
        [MarshalAs(UnmanagedType.U4)] private uint dwResetPeriod;
        [MarshalAs(UnmanagedType.LPStr)] private string lpRebootMsg;
        [MarshalAs(UnmanagedType.LPStr)] private string lpCommand;
        [MarshalAs(UnmanagedType.U4)] private int cActions;
        private IntPtr lpsaActions;

        public TimeSpan ResetPeriod => TimeSpan.FromSeconds(dwResetPeriod);

        public string RebootMsg => lpRebootMsg;

        public string Command => lpCommand;

        public int CountActions => cActions;

        public ScAction[] Actions => lpsaActions.MarshalUnmananagedArrayToStruct<ScAction>(cActions);

        /// <summary>
        /// This is the default, as reported by Windows.
        /// </summary>
        internal static ServiceFailureActionsInfo Default =
            new ServiceFailureActionsInfo {dwResetPeriod = 0, lpRebootMsg = null, lpCommand = null, cActions = 0, lpsaActions = IntPtr.Zero};

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFailureActionsInfo"/> class.
        /// </summary>
        internal ServiceFailureActionsInfo(TimeSpan resetPeriod, string rebootMessage, string restartCommand, IReadOnlyCollection<ScAction> actions)
        {
            dwResetPeriod = resetPeriod == TimeSpan.MaxValue ? uint.MaxValue : (uint) Math.Round(resetPeriod.TotalSeconds);
            lpRebootMsg = rebootMessage;
            lpCommand = restartCommand;
            cActions = actions?.Count ?? 0;

            if (null != actions)
            {
                lpsaActions = Marshal.AllocHGlobal(Marshal.SizeOf<ScAction>() * cActions);

                if (lpsaActions == IntPtr.Zero)
                {
                    throw new Exception(string.Format("Unable to allocate memory for service action, error was: 0x{0:X}", Marshal.GetLastWin32Error()));
                }
                
                var nextAction = lpsaActions;

                foreach (var action in actions)
                {
                    Marshal.StructureToPtr(action, nextAction, fDeleteOld: false);
                    nextAction = (IntPtr) (nextAction.ToInt64() + Marshal.SizeOf<ScAction>());
                }
            }
            else
            {
                lpsaActions = IntPtr.Zero;
            }
        }
    }
}