using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration.SettingsSources;
// ReSharper disable AccessToModifiedClosure

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class StaticSettingsSource_Tests
    {
        [Test]
        public void GetSettings_should_return_chosen_settings()
        {
            var settings = new CustomSettings(1);
            var source = new StaticSettingsSource<CustomSettings>(() => settings);
            source.GetSettings().Should().Match(s => (s as CustomSettings).Value == 1);
        }

        [Test]
        public void StaticSettingsSource_should_not_cache_settings()
        {
            var settings = new CustomSettings(1);
            var source = new StaticSettingsSource<CustomSettings>(() => settings);

            settings = new CustomSettings(2);
            source.GetSettings().Should().Match(s => (s as CustomSettings).Value == 2);
        }

        private class CustomSettings
        {
            public int Value { get; }

            public CustomSettings(int value)
            {
                Value = value;
            }
        }
    }
}