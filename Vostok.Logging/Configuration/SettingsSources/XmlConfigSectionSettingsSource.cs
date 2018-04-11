using System;
using System.Text;

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
                return settings;

            var settingsType = typeof (TSettings);
            var properties = settingsType.GetProperties();

            foreach (var property in properties)
            {
                if (section.Settings.TryGetValue(property.Name, out var value))
                {
                    if (!TryParse(value, property.PropertyType, out var parsedValue))
                        return default(TSettings);

                    property.SetValue(settings, parsedValue);
                }
                else
                {
                    return default(TSettings);
                }
            }

            return settings;
        }

        private static bool TryParse(string str, Type type, out object value)
        {
            value = null;

            if (type == typeof (string))
            {
                value = str;
                return true;
            }
            if (type == typeof (bool))
            {
                var result = bool.TryParse(str, out var innerValue);
                if (result)
                    value = innerValue;
                return result;
            }

            if (type == typeof(ConversionPattern))
            {
                value = ConversionPattern.FromString(str);
                return true;
            }

            if (type == typeof (Encoding))
            {
                try
                {
                    value = Encoding.GetEncoding(str);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        private readonly string sectionName;
    }
}