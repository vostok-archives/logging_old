using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEvent
    {
        public LogLevel Level { get; }

        public DateTime Timestamp { get; } // TODO(krait): DateTimeOffset

        public string MessageTemplate { get; }

        public IReadOnlyDictionary<string, object> Properties { get; }

        // TODO(krait): Exception

        public LogEvent(LogLevel level, string messageTemplate, IDictionary<string, object> properties, DateTime timestamp)
        {
            Level = level;
            MessageTemplate = messageTemplate;
            Properties = null;//properties;
            Timestamp = timestamp;
        }
    }
}