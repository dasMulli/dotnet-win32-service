namespace CSS.ServiceHost
{
    public enum ServiceStartType : uint
    {
        StartOnBoot = 0,
        StartOnSystemStart = 1,
        AutoStart = 2,
        StartOnDemand = 3,
        Disabled = 4
    }
}