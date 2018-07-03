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

        // CR(krait): Do you actually need to compare ConsoleLogSettings?
        public override int GetHashCode()
        {
            return (ConversionPattern?.GetHashCode() ?? 0) + EventsQueueCapacity * 3;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConsoleLogSettings);
        }

        private bool Equals(ConsoleLogSettings other)
        {
            if (other == null)
                return false;

            var conversionPatternsAreEqual = ConversionPattern == null && other.ConversionPattern == null ||
                                             ConversionPattern != null && ConversionPattern.Equals(other.ConversionPattern);

            return conversionPatternsAreEqual && EventsQueueCapacity == other.EventsQueueCapacity;
        }
    }
}