using System;

namespace Vostok.Logging.Configuration.SettingsSources
{
    internal class StaticSettingsSource<TSettings> : ISettingsSource<TSettings>
    {
        public StaticSettingsSource(Func<TSettings> sourceFunc)
        {
            this.sourceFunc = sourceFunc;
        }

        public TSettings GetSettings()
        {
            return sourceFunc();
        }

        private readonly Func<TSettings> sourceFunc;
    }
}