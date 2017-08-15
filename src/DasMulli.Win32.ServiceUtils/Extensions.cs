using System;
using System.Runtime.InteropServices;

namespace DasMulli.Win32.ServiceUtils
{
    internal static class Extensions
    {
        internal static T[] MarshalUnmananagedArrayToStruct<T>(this IntPtr unmanagedArray, int length)
        {
            var size = Marshal.SizeOf<T>();
            var mangagedArray = new T[length];

            for (var i = 0; i < length; i++)
            {
                var ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = Marshal.PtrToStructure<T>(ins);
            }

            return mangagedArray;
        }
    }
}