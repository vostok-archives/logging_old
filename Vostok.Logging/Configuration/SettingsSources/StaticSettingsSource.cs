using System;

namespace Vostok.Logging.Configuration.SettingsSources
{
    internal class StaticSettingsSource<TSettings> : ISettingsSource<TSettings> where TSettings : new()
    {
        public StaticSettingsSource(Func<TSettings> sourceFunc)
        {
            this.sourceFunc = sourceFunc;
        }

        public TSettings GetSettings()
        {
            var settings = sourceFunc();
            if (settings == null)
                return default;

            return CloneSettings(sourceFunc());
        }

        private static TSettings CloneSettings(TSettings settings)
        {
            var clonedSettings = new TSettings();

            foreach (var property in settings.GetType().GetProperties())
            {
                property.SetValue(clonedSettings, property.GetValue(settings));
            }

            return clonedSettings;
        }

        private readonly Func<TSettings> sourceFunc;
    }
}