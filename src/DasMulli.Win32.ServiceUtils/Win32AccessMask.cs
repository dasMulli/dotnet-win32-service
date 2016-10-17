using System;
using System.Diagnostics.CodeAnalysis;

namespace DasMulli.Win32.ServiceUtils
{
    [Flags]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "External API")]
    internal enum Win32AccessMask : uint
    {
        Delete = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac = 0x00040000,
        WriteOwner = 0x00080000,
        Synchronize = 0x00100000,

        StandardRightsRequired = 0x000F0000,

        StandardRightsRead = 0x00020000,
        StandardRightsWrite = 0x00020000,
        StandardRightsExecute = 0x00020000,

        StandardRightsAll = 0x001F0000,

        SpecificRightsAll = 0x0000FFFF,

        AccessSystemSecurity = 0x01000000,

        MaximumAllowed = 0x02000000,

        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,

        DesktopReadobjects = 0x00000001,
        DesktopCreatewindow = 0x00000002,
        DesktopCreatemenu = 0x00000004,
        DesktopHookcontrol = 0x00000008,
        DesktopJournalrecord = 0x00000010,
        DesktopJournalplayback = 0x00000020,
        DesktopEnumerate = 0x00000040,
        DesktopWriteobjects = 0x00000080,
        DesktopSwitchdesktop = 0x00000100,

        WinstaEnumdesktops = 0x00000001,
        WinstaReadattributes = 0x00000002,
        WinstaAccessclipboard = 0x00000004,
        WinstaCreatedesktop = 0x00000008,
        WinstaWriteattributes = 0x00000010,
        WinstaAccessglobalatoms = 0x00000020,
        WinstaExitwindows = 0x00000040,
        WinstaEnumerate = 0x00000100,
        WinstaReadscreen = 0x00000200,

        WinstaAllAccess = 0x0000037F
    }
}