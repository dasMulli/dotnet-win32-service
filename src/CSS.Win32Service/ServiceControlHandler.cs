using System;

namespace CSS.Win32Service
{
    internal delegate void ServiceControlHandler(ServiceControlCommand control, uint eventType, IntPtr eventData, IntPtr eventContext);
}