namespace Vostok.Logging.Configuration
{
    internal interface ILogConfigProvider<out TSettings> where TSettings : new()
    {
        TSettings Settings { get; }
    }
}