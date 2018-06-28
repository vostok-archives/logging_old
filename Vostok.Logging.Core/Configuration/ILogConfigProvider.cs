using System;

namespace Vostok.Logging.Core.Configuration
{
    // CR(krait): Let's switch the entire configuration infrastructure to vostok.configuration.
    internal interface ILogConfigProvider<out TSettings> : IDisposable where TSettings : new()
    {
        TSettings Settings { get; }
    }
}