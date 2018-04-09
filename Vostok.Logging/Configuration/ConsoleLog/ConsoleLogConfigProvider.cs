using System;
using System.Configuration;
using System.Threading;
using Vostok.Commons.Conversions;
using Vostok.Commons.ThreadManagment;

namespace Vostok.Logging.Configuration.ConsoleLog
{
    internal class ConsoleLogConfigProvider : IConsoleLogConfigProvider
    {
        public ConsoleLogSettings Settings { get; private set; }

        public ConsoleLogConfigProvider()
        {
            Settings = CreateNewSettings();
            ThreadRunner.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        UpdateCache();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    Thread.Sleep(updateCachePeriod);
                }
            });
        }

        private void UpdateCache()
        {
            var section = Section;
            if (section == null)
                return;

            var settingsWereChanged = TryUpdateConversionPattern(section);

            if (!settingsWereChanged)
                return;

            Settings = CreateNewSettings();
        }

        private ConsoleLogSettings CreateNewSettings()
        {
            return new ConsoleLogSettings
            {
                ConversionPattern = conversionPattern
            };
        }

        private bool TryUpdateConversionPattern(ConsoleLogConfigSection section)
        {
            var value = section.ConversionPattern.Value;
            if (conversionPattern.PatternStr.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                return false;

            conversionPattern = ConversionPattern.FromString(value);
            return true;
        }

        private static ConsoleLogConfigSection Section => (ConsoleLogConfigSection)ConfigurationManager.GetSection(ConsoleLogConfigSectionName);

        private ConversionPattern conversionPattern = ConversionPattern.Default;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();

        private const string ConsoleLogConfigSectionName = "consoleLogConfig";
    }
}