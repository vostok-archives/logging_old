using System;

namespace Vostok.Logging.Configuration.SettingsSources
{
    internal class StaticSettingsSource<TSettings> : ISettingsSource<TSettings>
    {
        public StaticSettingsSource(Func<TSettings> source)
        {
            this.source = source;
        }

        public TSettings GetSettings()
        {
            return source();
        }

        private readonly Func<TSettings> source;
    }
}