﻿using System;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Binders;
using Vostok.Configuration.Sources;

namespace Vostok.Logging.Core.Configuration
{
    internal class LogConfigProvider<TSettings> : ILogConfigProvider<TSettings> where TSettings : new()
    {
        public TSettings Settings => TryGetSettings();

        private TSettings TryGetSettings()
        {
            try
            {
                return configProvider.Get<TSettings>();
            }
            catch (Exception exception)
            {
                OnError(exception);
                return defaultSettings;
            }
        }

        public LogConfigProvider(string fileName, string sectionName) : this(new ScopedSource(new XmlFileSource(fileName), configurationTagName, sectionName)) { }

        public LogConfigProvider(string sectionName) : this(AppConfigFileName, sectionName) { }

        public LogConfigProvider(TSettings settings)
        {
            configProvider = GetConfiguredConfigProvider().SetManually(settings, true);
        }

        private LogConfigProvider(IConfigurationSource settingsSource)
        {
            configProvider = GetConfiguredConfigProvider().SetupSourceFor<TSettings>(settingsSource);
        }

        private static ConfigurationProvider GetConfiguredConfigProvider()
        {
            var binder = new DefaultSettingsBinder().WithDefaultParsers().WithCustomParser<ConversionPattern>(ConversionPattern.TryParse);
            var configProviderSettings = new ConfigurationProviderSettings { Binder = binder, OnError = OnError };
            return new ConfigurationProvider(configProviderSettings);
        }

        private static string AppConfigFileName => $"{AppDomain.CurrentDomain.FriendlyName}.config";

        private static void OnError(Exception exception)
        {
            Console.Out.WriteLine(exception);
        }

        private readonly IConfigurationProvider configProvider;
        private readonly TSettings defaultSettings = new TSettings();
        private const string configurationTagName = "configuration";
    }
}