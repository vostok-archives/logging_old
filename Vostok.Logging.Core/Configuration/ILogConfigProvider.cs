namespace Vostok.Logging.Core.Configuration
{
    // CR(krait): Let's switch the entire configuration infrastructure to vostok.configuration. FIXED
    internal interface ILogConfigProvider<out TSettings> where TSettings : new()
    {
        TSettings Settings { get; }
    }
}