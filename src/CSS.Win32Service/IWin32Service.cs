namespace CSS.ServiceHost
{
    public interface IWin32Service
    {
        string ServiceName { get; }

        void Start();

        void Stop();
    }
}