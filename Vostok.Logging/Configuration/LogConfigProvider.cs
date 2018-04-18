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

        public LogConfigProvider(string sectionName) 
            : this(new ConfigSectionSettingsSource<TSettings>(() => new XmlConfigSection(sectionName, $"{AppDomain.CurrentDomain.FriendlyName}.config"))) { }

        public LogConfigProvider(Func<TSettings> source) 
            : this(new StaticSettingsSource<TSettings>(source)) { }

        private LogConfigProvider(ISettingsSource<TSettings> settingsSource)
        {
            this.settingsSource = settingsSource;

            updateCacheThread = ThreadRunner.Run(() =>
            {
                while (!isDisposed)
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

        public void Dispose()
        {
            isDisposed = true;
        }

        private void UpdateCache()
        {
            var settings = settingsSource.GetSettings();
            if(settings == null)
                return;

            if (settings.AreValid())
                Settings = settings;
        }

        private bool isDisposed;

        private readonly ISettingsSource<TSettings> settingsSource;

        private readonly Thread updateCacheThread;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();
    }
}