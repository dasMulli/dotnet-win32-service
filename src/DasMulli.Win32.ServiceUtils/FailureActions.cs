using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DasMulli.Win32.ServiceUtils
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct FailureActions
    {
        [MarshalAs(UnmanagedType.U4)] private uint dwResetPeriod;
        [MarshalAs(UnmanagedType.LPStr)] private string lpRebootMsg;
        [MarshalAs(UnmanagedType.LPStr)] private string lpCommand;
        [MarshalAs(UnmanagedType.U4)] private uint cActions;
        private IntPtr lpsaActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureActions"/> class.
        /// </summary>
        internal FailureActions(TimeSpan resetPeriod, string rebootMessage, string restartCommand, IReadOnlyCollection<ScAction> actions)
        {

            dwResetPeriod = (uint) Math.Round(resetPeriod.TotalMilliseconds);
            lpRebootMsg = rebootMessage ?? "";
            lpCommand = restartCommand ?? "";
            cActions = (uint) actions.Count;

            lpsaActions = Marshal.AllocHGlobal(Marshal.SizeOf<ScAction>() * actions.Count);

            if (lpsaActions == IntPtr.Zero)
            {
                throw new Exception(String.Format("Unable to allocate memory for service action, error was: 0x{0:X}", Marshal.GetLastWin32Error()));
            }
            
            // Marshal.StructureToPtr(action, lpsaActions, false);
            IntPtr nextAction = lpsaActions;
            
            foreach (var action in actions)
            {
                Marshal.StructureToPtr(action, nextAction, fDeleteOld: false);
                nextAction = (IntPtr) (nextAction.ToInt64() + Marshal.SizeOf<ScAction>());
            }
        }
    }
}