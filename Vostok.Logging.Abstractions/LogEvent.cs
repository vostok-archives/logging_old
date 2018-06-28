using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Logging.Abstractions
{
    public sealed class LogEvent
    {
        public LogLevel Level { get; }

        public DateTimeOffset Timestamp { get; }

        public string MessageTemplate { get; }

        public IReadOnlyDictionary<string, object> Properties { get; }

        public Exception Exception { get; }

        // CR(krait): Let's switch to the specialized collection from commons, which should be copy-pasted here. The collection itself must be internal, of course.
        public LogEvent(LogLevel level, DateTimeOffset timestamp, string messageTemplate, IReadOnlyDictionary<string, object> properties = null, Exception exception = null)
        {
            Level = level;
            Timestamp = timestamp;
            MessageTemplate = messageTemplate;
            Properties = properties?.ToDictionary(p => p.Key, p => p.Value, StringComparer.CurrentCultureIgnoreCase);
            Exception = exception;
        }
    }
}