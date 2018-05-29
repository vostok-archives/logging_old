using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Core.Configuration.SettingsSources;

// ReSharper disable AccessToModifiedClosure

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class StaticSettingsSource_Tests
    {
        [Test]
        public void GetSettings_should_return_chosen_settings()
        {
            var settings = new CustomSettings { Value = 1 };
            var source = new StaticSettingsSource<CustomSettings>(() => settings);
            source.GetSettings().Should().Match(s => (s as CustomSettings).Value == 1);
        }

        [Test]
        public void StaticSettingsSource_should_not_cache_settings()
        {
            var settings = new CustomSettings { Value = 1 };
            var source = new StaticSettingsSource<CustomSettings>(() => settings);

            settings = new CustomSettings { Value = 2 };
            source.GetSettings().Should().Match(s => (s as CustomSettings).Value == 2);
        }

        [Test]
        public void StaticSettingsSource_should_return_null_source_settings_is_null()
        {
            var source = new StaticSettingsSource<CustomSettings>(() => null);

            source.GetSettings().Should().BeNull();
        }

        private class CustomSettings
        {
            public int Value { get; set; } = 3;
        }
    }
}