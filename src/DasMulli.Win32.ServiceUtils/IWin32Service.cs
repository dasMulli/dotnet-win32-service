namespace DasMulli.Win32.ServiceUtils
{
    public delegate void ServiceStoppedCallback();

    public interface IWin32Service
    {
        string ServiceName { get; }

        void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback);

        void Stop();
    }
}