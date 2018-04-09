using System.Configuration;

namespace Vostok.Logging.Configuration.ConsoleLog
{
    internal class ConsoleLogConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(ConversionPatternPropertyName)]
        public LogConfigElement ConversionPattern => (LogConfigElement)base[ConversionPatternPropertyName];

        private const string ConversionPatternPropertyName = "conversionPattern";
    }
}