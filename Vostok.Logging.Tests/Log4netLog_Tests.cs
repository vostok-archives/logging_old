using System;
using System.Linq;
using FluentAssertions;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Log4net;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class Log4netLog_Tests
    {
        [Test]
        public void Log4netLog_should_log_messages()
        {
            var messages = new[] { "Hello, World 1", "Hello, World 2" };
            
            log.Info(messages[0]);
            log.Info(messages[1]);

            appender.GetEvents().Select(x => x.RenderedMessage).Should().Equal(messages);
        }

        [Test]
        public void Log4netLog_should_use_context_as_logger_name()
        {
            log.ForContext("lalala").Info("bububu");
            appender.GetEvents().Single().LoggerName.Should().Be("lalala");
        }

        [Test]
        public void Log4netLog_with_hierarchy_should_use_context_as_logger_name()
        {
            SetupHierarhy("base");
            log.ForContext("lalala").ForContext("bububu").Info("msg2");
            appender.GetEvents().Single().LoggerName.Should().Be("base.lalala.bububu");
        }

        [Test]
        public void Log4netLog_should_create_events_with_correct_timestamp()
        {
            var timestamp = DateTimeOffset.UtcNow.AddDays(1);
            log.Log(new LogEvent(LogLevel.Info, timestamp, "lalala"));
            appender.GetEvents().Single().TimeStampUtc.Should().Be(timestamp.UtcDateTime);
        }

        [TestCaseSource(nameof(GetLevelsMap))]
        public void Log4netLog_should_translate_level_correctly(LogLevel level, Level log4netLevel)
        {
            log.Log(new LogEvent(level, DateTimeOffset.UtcNow, "lalala"));
            appender.GetEvents().Single().Level.Should().Be(log4netLevel);
        }

        private static object[][] GetLevelsMap()
        {
            return new[]
            {
                new object[] {LogLevel.Fatal, Level.Fatal},
                new object[] {LogLevel.Error, Level.Error},
                new object[] {LogLevel.Warn, Level.Warn},
                new object[] {LogLevel.Info, Level.Info},
                new object[] {LogLevel.Debug, Level.Debug},
            };
        }

        [SetUp]
        public void SetUp()
        {
            var repository = LogManager.GetAllRepositories().SingleOrDefault(x => x.Name == "test") ?? LogManager.CreateRepository("test");
            repository.ResetConfiguration();
            appender = new MemoryAppender();
            BasicConfigurator.Configure(repository, appender);
            log = new Log4netLog(LogManager.GetLogger("test", "root"));
        }

        private void SetupHierarhy(string rootContext)
        {
            log = new Log4netLog(LogManager.GetLogger("test", rootContext), true);
        }

        private MemoryAppender appender;
        private Abstractions.ILog log;
    }
}