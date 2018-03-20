using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEvent
    {
        public LogLevel Level { get; }

        public DateTimeOffset Timestamp { get; }

        public string MessageTemplate { get; }

        public IReadOnlyDictionary<string, object> Properties { get; }

        // TODO(krait): Exception
        public Exception Exception { get; }

        public LogEvent(LogLevel level, string messageTemplate, IReadOnlyDictionary<string, object> properties, Exception exception)
        {
            Level = level;
            MessageTemplate = messageTemplate;
            Properties = properties;
            Timestamp = DateTimeOffset.Now;
            Exception = exception;
        }
    }
}