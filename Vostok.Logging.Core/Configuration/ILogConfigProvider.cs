using System;

namespace Vostok.Logging.Core.Configuration
{
    internal interface ILogConfigProvider<out TSettings> : IDisposable where TSettings : new()
    {
        TSettings Settings { get; }
    }
}