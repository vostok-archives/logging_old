namespace Vostok.Logging.Core.Configuration
{
    internal interface ILogConfigProvider<out TSettings> where TSettings : new()
    {
        TSettings Settings { get; }
    }
}