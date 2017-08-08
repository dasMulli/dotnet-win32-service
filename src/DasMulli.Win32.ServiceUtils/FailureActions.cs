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
        [MarshalAs(UnmanagedType.U4)] private int cActions;
        private IntPtr lpsaActions;

        public ScAction[] Actions => MarshalUnmananagedArray2Struct<ScAction>(lpsaActions, cActions);

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureActions"/> class.
        /// </summary>
        internal FailureActions(TimeSpan resetPeriod, string rebootMessage, string restartCommand, IReadOnlyCollection<ScAction> actions)
        {
            dwResetPeriod = resetPeriod == TimeSpan.MaxValue ? uint.MaxValue : (uint) Math.Round(resetPeriod.TotalMilliseconds);
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

                // Marshal.StructureToPtr(action, lpsaActions, false);
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

        private static T[] MarshalUnmananagedArray2Struct<T>(IntPtr unmanagedArray, int length)
        {
            var size = Marshal.SizeOf<T>();
            var mangagedArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = Marshal.PtrToStructure<T>(ins);
            }

            return mangagedArray;
        }
    }
}