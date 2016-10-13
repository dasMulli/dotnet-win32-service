using System;

namespace CSS.ServiceHost
{
    internal delegate void ServiceControlHandler(ServiceControlCommand control, uint eventType, IntPtr eventData, IntPtr eventContext);
}