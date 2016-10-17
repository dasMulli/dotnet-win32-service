namespace DasMulli.Win32.ServiceUtils
{
    public interface IWin32Service
    {
        string ServiceName { get; }

        void Start(string[] startupArguments);

        void Stop();
    }
}