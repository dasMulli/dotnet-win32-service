namespace DasMulli.Win32.ServiceUtils
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Windows service that pause and continue between starting and stopping.
    /// After a call to <see cref="Pause"/>, the service can receive either <see cref="Continue"/> or <see cref="IWin32Service.Stop"/>.
    /// </summary>
    public interface IPausableWin32Service : IWin32Service
    {
        /// <summary>
        /// Pauses the service.
        /// Expect either <see cref="Continue"/> or <see cref="IWin32Service.Stop"/> to be called afterwards.
        /// </summary>
        void Pause();

        /// <summary>
        /// Continues (resumes) the service.
        /// </summary>
        void Continue();
    }
}
