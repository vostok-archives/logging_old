using System;
using System.Collections.Generic;
using Vostok.Logging.Abstractions.Flow;

namespace Vostok.Logging.Abstractions
{
    public sealed class LogEvent
    {
        public LogLevel Level { get; }

        public DateTimeOffset Timestamp { get; }

        public string MessageTemplate { get; }

        public IReadOnlyDictionary<string, object> Properties;

        public Exception Exception { get; }

        // CR(krait): Let's switch to the specialized collection from commons, which should be copy-pasted here. The collection itself must be internal, of course. FIXED
        public LogEvent(LogLevel level, DateTimeOffset timestamp, string messageTemplate, IReadOnlyDictionary<string, object> properties = null, Exception exception = null)
        {
            Level = level;
            Timestamp = timestamp;
            MessageTemplate = messageTemplate;
            Properties = ContextPropertiesSnapshot<string, object>.FromCollection(properties, StringComparer.CurrentCultureIgnoreCase);
            Exception = exception;
        }

        public LogEvent WithProperty<T>(string key, T value)
        {
            var properties = Properties == null 
                ? new ContextPropertiesSnapshot<string, object>(StringComparer.CurrentCultureIgnoreCase).Set(key, value)
                : ((ContextPropertiesSnapshot<string, object>)Properties).Set(key, value);

            return new LogEvent(Level, Timestamp, MessageTemplate, properties, Exception);
        }
    }
}