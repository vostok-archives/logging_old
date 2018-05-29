using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.ConsoleLog;
using Vostok.Logging.FileLog;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class SettingsValidator_Tests
    {
        [Test]
        public void Default_FileLogSettings_should_be_valid()
        {
            fileLogSettings.Validate().IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void Null_FileLogSettings_should_not_be_valid()
        {
            fileLogSettings = null;

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_ConversionPattern_should_not_be_valid()
        {
            fileLogSettings.ConversionPattern = null;

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_Encoding_should_not_be_valid()
        {
            fileLogSettings.Encoding = null;

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_null_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = null;

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_not_correct_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = "asdasf:asfdggasg?sadagfasdf";

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void FileLogSettings_with_absent_FilePath_should_not_be_valid()
        {
            fileLogSettings.FilePath = "C:\\HelloWorld\\Hello";

            fileLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void Default_ConsoleLogSettings_should_be_valid()
        {
            consoleLogSettings.Validate().IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void Null_ConsoleLogSettings_should_not_be_valid()
        {
            consoleLogSettings = null;

            consoleLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void ConsoleLogSettings_with_null_ConversionPattern_should_not_be_valid()
        {
            consoleLogSettings.ConversionPattern = null;

            consoleLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [SetUp]
        public void SetUp()
        {
            fileLogSettings = new FileLogSettings();
            consoleLogSettings = new ConsoleLogSettings();

            var logDirectoryPath = fileLogSettings.FilePath;
            if (!Directory.Exists(Path.GetDirectoryName(logDirectoryPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(logDirectoryPath));
        }

        private FileLogSettings fileLogSettings;
        private ConsoleLogSettings consoleLogSettings;
    }
}