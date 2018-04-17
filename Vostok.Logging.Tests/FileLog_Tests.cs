using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration;
using Vostok.Logging.Configuration.Settings;
using Vostok.Logging.Extensions;
using Vostok.Logging.Logs;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class FileLog_Tests
    {
        [Test]
        public void Test11()
        {
            var messages = new[] {"Hello, World 1", "Hello, World 2"};

            log.Info(messages[0]);
            log.Info(messages[1]);
            Thread.Sleep(300);

            ReadAllLines(ToFullFileName(logFileName)).Should().BeEquivalentTo(messages);
        }

        [Test]
        public void Test12()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };

            log.Info(messages[0]);
            Thread.Sleep(30000);
            settings.AppendToFile = false;
            Thread.Sleep(30000);
            log.Info(messages[1]);
            Thread.Sleep(30000);

            ReadAllLines(ToFullFileName(logFileName)).Should().BeEquivalentTo(messages[1]);
        }

        [SetUp]
        public void SetUp()
        {
            logFileName = $"{Guid.NewGuid().ToString().Substring(0, 8)}.log";
            settings = new FileLogSettings
            {
                FilePath = logFileName,
                ConversionPattern = ConversionPattern.FromString("%m%n")
            };
            log = new FileLog();
            log.Configure(() => settings);
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(logFileName))
                File.Delete(logFileName);
        }

        private static string ToFullFileName(string fileName)
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

        private string logFileName;
        private FileLogSettings settings;
        private FileLog log;
    }
}