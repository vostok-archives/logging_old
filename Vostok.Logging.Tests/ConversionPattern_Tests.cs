using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
// ReSharper disable UseStringInterpolation

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class ConversionPattern_Tests
    {
        [Test]
        public void Format_should_return_correct_string_for_default_pattern([Values] LogLevel level)
        {
            var pattern = ConversionPattern.Default;
            var exception = new Exception("AnyException");
            var @event = new LogEvent(level, DateTimeOffset.UtcNow, "Hello, World", exception).WithProperty("prop", "ccc");

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

        [Test]
        public void Format_should_return_correct_string_for_level_key_in_template([Values] LogLevel level)
        {
            var pattern = ConversionPattern.FromString("%l");
            var @event = GenerateEvent(level);

            pattern.Format(@event).Should().Be($"{level}");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_level_key_in_template([Values] LogLevel level)
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

            pattern.Format(@event).Should().Be(string.Empty);
        }

        [Test]
        public void Format_should_return_correct_string_for_all_properties_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%p");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("[properties: prop = ccc]");
        }

        [Test]
        public void Format_should_return_correct_string_for_upper_cased_all_properties_key_in_template()
        {
            var pattern = ConversionPattern.FromString("%P");
            var @event = GenerateEvent(new Dictionary<string, object> { { "prop", "ccc" } });

            pattern.Format(@event).Should().Be("[properties: prop = ccc]");
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



        [Test]
        public void Test()
        {
            var dateTime = DateTimeOffset.UtcNow;
            const LogLevel level = LogLevel.Info;
            const string prefix = "p";
            const string message = "Hello, World";
            var exception = new Exception("AnyException");
            const string property = "value";
            var properties = new Dictionary<string, object> { { "prefix", new []{ prefix } } , { "prop", property }};

            var pattern = ConversionPattern.FromString("a%da%la%xa%ma%ea%pa%p(prop)a%n");

            var @event = GenerateEvent(dateTime, message, exception, properties);
            var template = string.Format("a{0:HH:mm:ss zzz}a{1}a[{2}]a{3}a{4}a[properties: {5}]a{6}a\r\n",
                dateTime, 
                level,
                prefix,
                message,
                exception, 
                string.Join(", ", properties.Select(p => $"{ p.Key} = { p.Value}")),
                property);
            pattern.Format(@event).Should().Be(template);
        }

        [Test]
        public void Test2()
        {
            var dateTime = DateTimeOffset.UtcNow;
            const LogLevel level = LogLevel.Info;
            const string prefix = null;
            const string message = null;
            var exception = new Exception("AnyException");
            const string property = "value";
            var properties = new Dictionary<string, object> { { "prefix", new[] { prefix } }, { "prop", property } };

            var pattern = ConversionPattern.FromString("a%da%la%xa%ma%ea%pa%p(prop)a%n");

            var @event = GenerateEvent(dateTime, message, exception, properties);
            var template = string.Format("a{0:HH:mm:ss zzz}a{1}a[{2}]a{3}a{4}a[properties: {5}]a{6}a\r\n",
                dateTime,
                level,
                prefix,
                message,
                exception,
                string.Join(", ", properties.Select(p => $"{ p.Key} = { p.Value}")),
                property);
            pattern.Format(@event).Should().Be(template);
        }

        private static LogEvent GenerateEvent(LogLevel level = LogLevel.Info)
        {
            return new LogEvent(level, DateTimeOffset.UtcNow, null);
        }

        private static LogEvent GenerateEvent(Exception exception)
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, null, exception);
        }

        private static LogEvent GenerateEvent(IReadOnlyDictionary<string, object> properties)
        {
            return properties.Aggregate(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, null), (e, prop) => e.WithProperty(prop.Key, prop.Value));
        }

        private static LogEvent GenerateEvent(string messageTemplate, IReadOnlyDictionary<string, object> properties = null)
        {
            var @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate);
            return properties?.Aggregate(@event, (e, prop) => e.WithProperty(prop.Key, prop.Value)) ?? @event;
        }

        private static LogEvent GenerateEvent(DateTimeOffset timestamp, string messageTemplate, Exception exception, IReadOnlyDictionary<string, object> properties)
        {
            var @event = new LogEvent(LogLevel.Info, timestamp, messageTemplate, exception);
            return properties?.Aggregate(@event, (e, prop) => e.WithProperty(prop.Key, prop.Value)) ?? @event;
        }
    }
}