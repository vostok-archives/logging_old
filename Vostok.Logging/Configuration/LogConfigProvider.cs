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

        public LogConfigProvider(string sectionName, ILogSettingsValidator<TSettings> settingsValidator)
            : this(new ConfigSectionSettingsSource<TSettings>(() => new XmlConfigSection(sectionName, $"{AppDomain.CurrentDomain.FriendlyName}.config")),
                settingsValidator) { }

        public LogConfigProvider(Func<TSettings> source, ILogSettingsValidator<TSettings> settingsValidator) 
            : this(new StaticSettingsSource<TSettings>(source), 
                settingsValidator) { }

        private LogConfigProvider(ISettingsSource<TSettings> settingsSource, ILogSettingsValidator<TSettings> settingsValidator)
        {
            this.settingsSource = settingsSource;
            this.settingsValidator = settingsValidator;

            TryUpdateCache();

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
                {
                    if (Settings == null)
                        Settings = new TSettings();

                    return;
                }

                var validationResult = settingsValidator.Validate(settings);
                validationResult.EnsureSuccess();

                Settings = settings;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                if (Settings == null)
                    Settings = new TSettings();
            }
        }

        private bool isDisposed;

        private readonly ISettingsSource<TSettings> settingsSource;
        private readonly ILogSettingsValidator<TSettings> settingsValidator;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();
    }
}