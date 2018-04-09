using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class ConversionPattern_Tests
    {
        [Test]
        public void Test1()
        {
            var pattern = ConversionPattern.Default;
            pattern.PatternStr.Should().Be("{0} {1} {2} {3} {4}");
        }

        [Test]
        public void Test2()
        {
            var pattern = ConversionPattern.Default;
            var @event = GenerateEvent();
            pattern.Format(@event).Should().Be($"{@event.Timestamp:HH:mm:ss zzz} {@event.Level} {@event.MessageTemplate} {@event.Exception} {Environment.NewLine}");
        }

        private static LogEvent GenerateEvent()
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message", new Dictionary<string, object>{{"", new object()}}, new Exception());
        }
    }
}