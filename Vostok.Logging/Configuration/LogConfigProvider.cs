using System;
using System.Threading;
using Vostok.Commons.Conversions;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration.Sections;
using Vostok.Logging.Configuration.SettingsSources;
using Vostok.Logging.Extensions;

namespace Vostok.Logging.Configuration
{
    internal class LogConfigProvider<TSettings> : ILogConfigProvider<TSettings> where TSettings : new()
    {
        public TSettings Settings { get; private set; } = new TSettings();

        private readonly Guid id = Guid.NewGuid();

        public LogConfigProvider(string sectionName) 
            : this(new ConfigSectionSettingsSource<TSettings>(() => new XmlConfigSection(sectionName, $"{AppDomain.CurrentDomain.FriendlyName}.config"))) { }

        public LogConfigProvider(Func<TSettings> source) 
            : this(new StaticSettingsSource<TSettings>(source)) { }

        private LogConfigProvider(ISettingsSource<TSettings> settingsSource)
        {
            this.settingsSource = settingsSource;

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
            var settings = settingsSource.GetSettings();
            if(settings == null)
                return;

            if (settings.AreValid())
                Settings = settings;
        }

        private readonly ISettingsSource<TSettings> settingsSource;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();
    }
}