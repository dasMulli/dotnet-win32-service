using System;

namespace DasMulli.Win32.ServiceUtils
{
    [Flags]
    internal enum ServiceControlAccessRights : uint
    {
        QueryConfig = 0x00001,
        ChangeConfig = 0x00002,
        QueryStatus = 0x00004,
        EnumerateDependents = 0x00008,
        Start = 0x00010,
        Stop = 0x00020,
        PauseContinue = 0x00040,
        Interrogate = 0x00080,
        UserDefinedControl = 0x00100,

        All = Win32AccessMask.StandardRightsRequired 
              | QueryConfig
              | ChangeConfig
              | QueryStatus
              | EnumerateDependents
              | Start
              | Stop
              |  PauseContinue
              | Interrogate
              | UserDefinedControl
    }
}