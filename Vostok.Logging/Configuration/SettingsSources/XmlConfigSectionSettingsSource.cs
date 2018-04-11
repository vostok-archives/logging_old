using System;
using System.Collections.Generic;
using System.Text;
using Vostok.Logging.Configuration.Parsing;

namespace Vostok.Logging.Configuration.SettingsSources
{
    internal class XmlConfigSectionSettingsSource<TSettings> : ISettingsSource<TSettings>  where TSettings: new()
    {
        public XmlConfigSectionSettingsSource(string sectionName)
        {
            this.sectionName = sectionName;
        }

        public TSettings GetSettings()
        {
            var settings = new TSettings();

            var section = new XmlConfigSection(sectionName);
            if (section.Settings.Count == 0)
                return default(TSettings);

            var properties = typeof(TSettings).GetProperties();
            foreach (var property in properties)
            {
                if (section.Settings.TryGetValue(property.Name, out var setting))
                {
                    if (!inlineParsers.TryGetValue(property.PropertyType, out var parser))
                        return default(TSettings);

                    if (!parser.TryParse(setting, out var parsedSetting))
                        return default(TSettings);

                    property.SetValue(settings, parsedSetting);
                    continue;
                }

                return default(TSettings);
            }

            return settings;
        }

        private readonly string sectionName;

        private readonly Dictionary<Type, IInlineParser> inlineParsers = new Dictionary<Type, IInlineParser>
        {
            {typeof(string), new InlineParser<string>(StringParser.TryParse)},
            {typeof(ConversionPattern), new InlineParser<ConversionPattern>(ConversionPattern.TryParse)},
            {typeof(bool), new InlineParser<bool>(bool.TryParse)},
            {typeof(Encoding), new InlineParser<Encoding>(EncodingParser.TryParse)}
        };
    }
}