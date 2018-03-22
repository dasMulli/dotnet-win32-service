using System;
using System.Collections.Generic;
using System.Text;

namespace DasMulli.Win32.ServiceUtils
{
    public interface IPausableWin32Service : IWin32Service
    {
        /// <summary>
        /// Pauses the service.
        /// </summary>
        void Pause();

        /// <summary>
        /// Continues/Resumes the service.
        /// </summary>
        void Continue();
    }
}
