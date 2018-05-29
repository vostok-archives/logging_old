using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration;
using Vostok.Logging.Extensions;
using Vostok.Logging.FileLog;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class FileLog_Tests
    {
        [Test]
        public void FileLog_should_work_correctly_for_default_settings()
        {
            var messages = new[] {"Hello, World 1", "Hello, World 2"};

            log.Info(messages[0]);
            log.Info(messages[1]);
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages);
        }
        [Test]
        public void FileLog_should_switch_logfile_if_FilePath_was_changed()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            log.Info(messages[0]);
            WaitForOperationCanceled();

            var oldFilePath = settings.FilePath;
            settings.FilePath = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log";
            UpdateSettings(settings);

            log.Info(messages[1]);
            WaitForOperationCanceled();

            createdFiles.Add(oldFilePath);
            createdFiles.Add(settings.FilePath);

            ReadAllLines(oldFilePath).Should().BeEquivalentTo(messages[0]);
            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages[1]);
        }

        [Test]
        public void FileLog_should_rewrite_logfile_if_AppendToFile_was_enabled()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            log.Info(messages[0]);
            WaitForOperationCanceled();

            settings.AppendToFile = false;
            UpdateSettings(settings);

            log.Info(messages[1]);
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages[1]);
        }

        [Test]
        public void FileLog_should_use_date_in_logfile_name_if_EnableRolling_was_enabled()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            log.Info(messages[0]);
            WaitForOperationCanceled();

            settings.EnableRolling = true;
            UpdateSettings(settings);

            log.Info(messages[1]);
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);
            createdFiles.Add(AddDate(settings.FilePath));

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages[0]);
            ReadAllLines(AddDate(settings.FilePath)).Should().BeEquivalentTo(messages[1]);
        }

        [Test]
        public void FileLog_should_change_notes_format_if_ConversionPattern_was_updated()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            log.Info(messages[0], new { trace = 134 });
            WaitForOperationCanceled();

            settings.ConversionPattern = ConversionPattern.FromString("%l %p(trace) %m%n");
            UpdateSettings(settings);

            log.Info(messages[1], new { trace = 134 });
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages[0], $"Info 134 {messages[1]}");
        }

        [Test]
        public void FileLog_should_switch_notes_encoding_if_Encoding_was_updated()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            var secondMessageBytes = Encoding.UTF8.GetBytes($"{messages[1]}{Environment.NewLine}");
            var convertedSecondMessageBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, secondMessageBytes);
            var convertedSecondMessage = Encoding.UTF8.GetString(convertedSecondMessageBytes);

            log.Info(messages[0]);
            WaitForOperationCanceled();

            settings.Encoding = Encoding.Unicode;
            UpdateSettings(settings);

            log.Info(messages[1]);
            WaitForOperationCanceled();

            createdFiles.Add(settings.FilePath);

            ReadAllLines(settings.FilePath).Should().BeEquivalentTo(messages[0], convertedSecondMessage);
        }

        [SetUp]
        public void SetUp()
        {
            settings = new FileLogSettings
            {
                FilePath = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log",
                ConversionPattern = ConversionPattern.FromString("%m%n"),
                EnableRolling = false,
                AppendToFile = true,
                Encoding = Encoding.UTF8
            };

            UpdateSettings(settings);
        }

        [TearDown]
        public void TearDown()
        {
            Task.Run(() =>
                {
                    settings.FilePath = "temp";
                    settings.EnableRolling = false;
                    settings.ConversionPattern = ConversionPattern.FromString(string.Empty);
                    UpdateSettings(settings);
                    log.Info(string.Empty);
                });

            createdFiles.ForEach(DeleteFile);
            createdFiles.Clear();
        }

        private static void DeleteFile(string fileName)
        {
            while (true)
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    break;
                }
                catch (Exception)
                {
                    WaitForOperationCanceled();
                }
            }
        }

        private static void UpdateSettings(FileLogSettings settingsPatch)
        {
            FileLog.FileLog.Configure(() => settingsPatch);
            WaitForOperationCanceled();
        }

        private static void WaitForOperationCanceled()
        {
            Thread.Sleep(300);
        }

        private static string AddDate(string fileName)
        {
            return $"{fileName}{DateTimeOffset.UtcNow.Date:yyyy.MM.dd}";
        }

        private static IEnumerable<string> ReadAllLines(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd().Split(Environment.NewLine).Where(s => !string.IsNullOrEmpty(s));
            }
        }

        private FileLogSettings settings;

        private readonly FileLog.FileLog log = new FileLog.FileLog();
        private readonly List<string> createdFiles = new List<string>(2);
    }
}