using System;
using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [Flags]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal enum ServiceControlManagerAccessRights : uint
    {
        Connect = 0x00001,
        CreateService = 0x00002,
        EnumerateServices = 0x00004,
        LockServiceDatabase = 0x00008,
        QueryLockStatus = 0x00010,
        ModifyBootConfig = 0x00020,

        All = Win32AccessMask.StandardRightsRequired |
              Connect |
              CreateService |
              EnumerateServices |
              LockServiceDatabase |
              QueryLockStatus |
              ModifyBootConfig,

        GenericRead = Win32AccessMask.StandardRightsRequired |
                      EnumerateServices |
                      QueryLockStatus,

        GenericWrite = Win32AccessMask.StandardRightsRequired |
                       CreateService |
                       ModifyBootConfig,

        GenericExecute = Win32AccessMask.StandardRightsRequired |
                         Connect |
                         LockServiceDatabase,

        GenericAll = All
    }
}