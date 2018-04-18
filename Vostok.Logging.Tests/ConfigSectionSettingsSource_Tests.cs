using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.Configuration.Sections;
using Vostok.Logging.Configuration.Settings;
using Vostok.Logging.Configuration.SettingsSources;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class ConfigSectionSettingsSource_Tests
    {
        [Test]
        public void GetSettings_should_return_null_if_section_is_null()
        {
            configSection = null;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_return_correct_settings_if_all_fields_were_successfully_parsed()
        {
            var settings = source.GetSettings();

            settings.Should().NotBeNull();
            settings.FilePath.Should().Be(defaultSettings.FilePath);
            settings.ConversionPattern.Should().Be(defaultSettings.ConversionPattern);
            settings.AppendToFile.Should().Be(defaultSettings.AppendToFile);
            settings.EnableRolling.Should().Be(defaultSettings.EnableRolling);
            settings.Encoding.Should().Be(defaultSettings.Encoding);
        }

        [Test]
        public void GetSettings_should_return_settings_with_default_string_field_if_it_is_absent()
        {
            settingsDict.Remove("filePath");

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
            settings.FilePath.Should().Be(defaultSettings.FilePath);
        }

        [Test]
        public void GetSettings_should_return_null_if_string_field_is_null()
        {
            settingsDict["filePath"] = null;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_not_return_null_if_string_field_is_empty_string()
        {
            settingsDict["filePath"] = string.Empty;

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
        }

        [Test]
        public void GetSettings_should_return_settings_with_default_conversionPattern_field_if_it_is_absent()
        {
            settingsDict.Remove("conversionPattern");

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
            settings.ConversionPattern.Should().Be(defaultSettings.ConversionPattern);
        }

        [Test]
        public void GetSettings_should_return_null_if_conversionPattern_field_is_null()
        {
            settingsDict["conversionPattern"] = null;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_not_return_null_if_conversionPattern_field_is_empty_string()
        {
            settingsDict["conversionPattern"] = string.Empty;

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
        }

        [Test]
        public void GetSettings_should_return_settings_with_default_bool_field_if_it_is_absent()
        {
            settingsDict.Remove("appendToFile");

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
            settings.AppendToFile.Should().Be(defaultSettings.AppendToFile);
        }

        [Test]
        public void GetSettings_should_return_null_if_bool_field_is_null()
        {
            settingsDict["appendToFile"] = null;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_return_null_if_bool_field_can_not_be_parsed()
        {
            settingsDict["appendToFile"] = string.Empty;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_return_settings_with_default_encoding_value_if_it_is_absent()
        {
            settingsDict.Remove("encoding");

            var settings = source.GetSettings();

            settings.Should().NotBeNull();
            settings.Encoding.Should().Be(defaultSettings.Encoding);
        }

        [Test]
        public void GetSettings_should_return_null_if_encoding_field_is_null()
        {
            settingsDict["encoding"] = null;

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [Test]
        public void GetSettings_should_return_null_if_encoding_field_can_not_be_parsed()
        {
            settingsDict["encoding"] = "utf-2008";

            var settings = source.GetSettings();

            settings.Should().BeNull();
        }

        [SetUp]
        public void SetUp()
        {
            settingsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "filePath", "C:\\logs\\log"},
                { "conversionPattern", "%d %l %m %e %n" },
                { "appendToFile", "true"},
                { "enableRolling", "true"},
                { "encoding", "utf-8" }
            };
            configSection = Substitute.For<IConfigSection>();
            configSection.Settings.Returns(settingsDict);
            source = new ConfigSectionSettingsSource<FileLogSettings>(() => configSection);
        }

        private IConfigSection configSection;
        private ConfigSectionSettingsSource<FileLogSettings> source;
        private Dictionary<string, string> settingsDict;
        private readonly FileLogSettings defaultSettings = new FileLogSettings();
    }
}