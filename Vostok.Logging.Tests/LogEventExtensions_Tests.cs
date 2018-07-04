using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Tests
{
    internal class LogEvent_Tests
    {
        [Test]
        public void WithProperty_should_add_absent_property_to_log_event_properties()
        {
            var @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message", new Dictionary<string, object>{{"A", 1}});
            @event  = @event.WithProperty("B", 2);
            @event.Properties.Should().BeEquivalentTo(new Dictionary<string, object> { { "A", 1 }, { "B", 2 } });
        }

        [Test]
        public void WithProperty_should_rewrite_existed_property()
        {
            var @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message", new Dictionary<string, object> { { "A", 1 } });
            @event = @event.WithProperty("A", 2);
            @event.Properties.Should().BeEquivalentTo(new Dictionary<string, object> { { "A", 2 } });
        }

        [Test]
        public void WithProperty_should_rewrite_property_if_such_property_in_other_case_exists_regardless_collection_is_ignorecased_or_not()
        {
            var @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message", new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase) { { "a", 1 } });
            @event = @event.WithProperty("A", 2);
            @event.Properties.Should().BeEquivalentTo(new Dictionary<string, object> { { "A", 2 } });

            @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message", new Dictionary<string, object> { { "a", 1 } });
            @event = @event.WithProperty("A", 2);
            @event.Properties.Should().BeEquivalentTo(new Dictionary<string, object> { { "A", 2 } });
        }

        [Test]
        public void WithProperty_should_create_new_properties_if_event_has_no_properties()
        {
            var @event = new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, "message");
            @event = @event.WithProperty("A", 1);
            @event.Properties.Should().BeEquivalentTo(new Dictionary<string, object> { { "A", 1 } });
        }
    }
}