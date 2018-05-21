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

        public LogConfigProvider(Func<TSettings> source) : this(new StaticSettingsSource<TSettings>(source))
        {
            TryUpdateCache();
        }

        private LogConfigProvider(ISettingsSource<TSettings> settingsSource)
        {
            this.settingsSource = settingsSource;

            ThreadRunner.Run(() =>
            {
                while (!isDisposed)
                {
                    TryUpdateCache();
                    Thread.Sleep(updateCachePeriod);
                }
            });
        }

        public void Dispose()
        {
            isDisposed = true;
        }

        private void TryUpdateCache()
        {
            try
            { 
                var settings = settingsSource.GetSettings();
                if (settings == null)
                    return;

                if (settings.AreValid())
                    Settings = settings;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool isDisposed;

        private readonly ISettingsSource<TSettings> settingsSource;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();
    }
}