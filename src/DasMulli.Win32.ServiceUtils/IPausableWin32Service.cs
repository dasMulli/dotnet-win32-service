namespace DasMulli.Win32.ServiceUtils
{
    /// <inheritdoc />
    /// <summary>
    /// Interface to allow implementing windows services that can start and stop as well as pause and continue in between.
    /// Note that after a call to Pause(), the service can receive either Continue() or Stop()
    /// </summary>
    public interface IPausableWin32Service : IWin32Service
    {
        /// <summary>
        /// Pauses the service.
        /// Expect either Continue() or Stop() to be called afterwards.
        /// </summary>
        void Pause();

        /// <summary>
        /// Continues/Resumes the service.
        /// </summary>
        void Continue();
    }
}
