using System;

namespace Vostok.Logging.Configuration
{
    internal interface ILogConfigProvider<out TSettings> : IDisposable where TSettings : new()
    {
        TSettings Settings { get; }
    }
}