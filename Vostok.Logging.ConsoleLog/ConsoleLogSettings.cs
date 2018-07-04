// ReSharper disable NonReadonlyMemberInGetHashCode
using Vostok.Configuration.Abstractions;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.ConsoleLog
{
    [ValidateBy(typeof(ConsoleLogSettingsValidator))]
    public class ConsoleLogSettings
    {
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;
        public int EventsQueueCapacity { get; set; } = 10000;
    }
}