using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration.Settings;
using Vostok.Logging.Extensions;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class SettingsValidator_Tests
    {
        [Test]
        public void Default_FileLogSettings_should_be_valid()
        {
            fileLogSettings.AreValid().Should().BeTrue();
        }

        [Test]
        public void Null_FileLogSettings_should_not_be_valid()
        {
            fileLogSettings = null;

            fileLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_ConversionPattern_should_not_be_valid()
        {
            fileLogSettings.ConversionPattern = null;

            fileLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_Encoding_should_not_be_valid()
        {
            fileLogSettings.Encoding = null;

            fileLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = null;

            fileLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_not_correct_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = "asdasf:asfdggasg?sadagfasdf";

            fileLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_absent_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = "C:\\HelloWorld\\Hello";

            fileLogSettings.AreValid().Should().BeFalse();
        }


        [Test]
        public void Default_ConsoleLogSettings_should_be_valid()
        {
            consoleLogSettings.AreValid().Should().BeTrue();
        }

        [Test]
        public void Null_ConsoleLogSettings_should_not_be_valid()
        {
            consoleLogSettings = null;

            consoleLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void ConsoleLogSettings_with_null_ConversionPattern_should_not_be_valid()
        {
            consoleLogSettings.ConversionPattern = null;

            consoleLogSettings.AreValid().Should().BeFalse();
        }

        [Test]
        public void NotSupportedSettings_should_not_be_valid()
        {
            notSupportedSettings.AreValid().Should().BeFalse();
        }

        [SetUp]
        public void SetUp()
        {
            fileLogSettings = new FileLogSettings();
            consoleLogSettings = new ConsoleLogSettings();
            notSupportedSettings = new NotSupportedSettings();

            var logDirectoryPath = fileLogSettings.FilePath;
            if(!Directory.Exists(Path.GetDirectoryName(logDirectoryPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(logDirectoryPath));
        }

        private FileLogSettings fileLogSettings;
        private ConsoleLogSettings consoleLogSettings;
        private NotSupportedSettings notSupportedSettings;

        private class NotSupportedSettings
        {
            public int NotSupportedSetting { get; set; } = 4;
        }
    }
}