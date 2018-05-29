using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Core;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class ConversionPattern_Tests
    {
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void Format_should_return_correct_string_for_default_pattern(LogLevel level)
        {
            var pattern = ConversionPattern.Default;
            var exception = new Exception("AnyException");
            var @event = new LogEvent(level, DateTimeOffset.UtcNow, "aaa{prop}bbb", new Dictionary<string, object>{{"prop", "ccc"}}, exception);

            var timestamp = @event.Timestamp.ToString("HH:mm:ss zzz");
            var formattedMessage = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);

            pattern.Format(@event).Should().Be($"{timestamp} {level} {formattedMessage} {exception} {Environment.NewLine}");
        }

        [Test]
        public void Format_should_return_correct_string_for_date_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%d");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be($"{@event.Timestamp:HH:mm:ss zzz}");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_date_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%D");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be($"{@event.Timestamp:HH:mm:ss zzz}");
        }

        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void Format_should_return_correct_string_for_level_key_in_template(LogLevel level)
        {
            var pattern = ConversionPattern.FromString("%l");
            var @event = GenerateEvent(level);

            pattern.Format(@event).Should().Be($"{level}");
        }

        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Fatal)]
        public void Format_should_return_correct_string_for_upper_cased_level_key_in_template(LogLevel level)
        {
            var pattern = ConversionPattern.FromString("%L");
            var @event = GenerateEvent(level);

            pattern.Format(@event).Should().Be($"{level}");
        }

        [Test]
        public void Format_should_return_correct_string_for_message_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%m");
            var @event = GenerateEvent("aaa{prop}bbb");

            pattern.Format(@event).Should().Be("aaa{prop}bbb");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_message_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%M");
            var @event = GenerateEvent("aaa{prop}bbb");

            pattern.Format(@event).Should().Be("aaa{prop}bbb");
        }

        [Test]
        public void Format_should_not_replace_placeholders_in_message_tamplate_if_needed_properties_are_absent()
        {
            var pattern = ConversionPattern.FromString("%m");
            var @event = GenerateEvent("aaa{prop}bbb", new Dictionary<string, object> { { "other", "ccc" } });

            pattern.Format(@event).Should().Be("aaa{prop}bbb");
        }

        [Test]
        public void Format_should_replace_placeholders_in_message_tamplate_if_needed_properties_are_exists()
        {
            var pattern = ConversionPattern.FromString("%m");
            var @event = GenerateEvent("aaa{prop}bbb", new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("aaacccbbb");
        }

        [Test]
        public void Format_should_replace_placeholders_in_message_tamplate_if_needed_properties_are_exists_in_other_case()
        {
            var pattern = ConversionPattern.FromString("%m");
            var @event = GenerateEvent("aaa{prop}bbb", new Dictionary<string, object> { { "ProP", "ccc" } });

            pattern.Format(@event).Should().Be("aaacccbbb");
        }

        [Test]
        public void Format_should_return_correct_string_for_single_property_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%p(prop)");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("ccc");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_single_property_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%P(prop)");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("ccc");
        }

        [Test]
        public void Format_should_replace_single_property_key_if_needed_property_found_in_other_key()
        {
            var pattern = ConversionPattern.FromString("%p(ProP)");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("ccc");
        }

        [Test]
        public void Format_should_not_replace_single_property_key_if_needed_property_is_absent()
        {
            var pattern = ConversionPattern.FromString("%p(prop)");
            var @event = GenerateEvent(new Dictionary<string, object> { { "other", "ccc" } });

            pattern.Format(@event).Should().Be("%p(prop)");
        }

        [Test]
        public void Format_should_return_correct_string_for_all_properties_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%p");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("ccc");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_all_properties_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%P");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("ccc");
        }

        [Test]
        public void Format_should_not_replace_all_properties_key_if_properties_are_empty()
        {
            var pattern = ConversionPattern.FromString("%p");
            var @event = GenerateEvent(new Dictionary<string, object>());

            pattern.Format(@event).Should().Be(string.Empty);
        }

        [Test]
        public void Format_should_not_replace_all_properties_key_if_properties_are_null()
        {
            var pattern = ConversionPattern.FromString("%p");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be(string.Empty);
        }

        [Test]
        public void Format_should_return_correct_string_for_exception_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%e");
            var exception = new Exception("AnyException");
            var @event = GenerateEvent(exception);

            pattern.Format(@event).Should().Be(exception.ToString());
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_exception_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%E");
            var exception = new Exception("AnyException");
            var @event = GenerateEvent(exception);

            pattern.Format(@event).Should().Be(exception.ToString());
        }

        [Test]
        public void Format_should_not_replece_exception_key_if_exception_is_null()
        {
            var pattern = ConversionPattern.FromString("%e");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be(string.Empty);
        }

        [Test]
        public void Format_should_return_correct_string_for_newline_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%n");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be(Environment.NewLine);
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_newline_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%N");
            var @event = GenerateEvent();

            pattern.Format(@event).Should().Be(Environment.NewLine);
        }

        private static LogEvent GenerateEvent(LogLevel level = LogLevel.Info)
        {
            return new LogEvent(level, DateTimeOffset.UtcNow, null);
        }

        private static LogEvent GenerateEvent(Exception exception)
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, null, null, exception);
        }

        private static LogEvent GenerateEvent(IReadOnlyDictionary<string, object> properties)
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, null, properties);
        }

        private static LogEvent GenerateEvent(string messageTemplate, IReadOnlyDictionary<string, object> properties = null)
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, properties);
        }
    }
}