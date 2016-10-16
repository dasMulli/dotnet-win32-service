namespace CSS.Win32Service
{
    public interface IWin32Service
    {
        string ServiceName { get; }

        void Start(string[] startupArguments);

        void Stop();
    }
}