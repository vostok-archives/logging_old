using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Logging
{
    public static class LogExtensions
    {
        // TODO(krait): [Obsolete] Info(message, exception)
        // TODO(krait): Info(exception, message, ...)

        // TODO(krait): Support IFormattable with invariant culture

        public static void Info(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, message));
        }

        public static void Info<T>(this ILog log, string messageTemplate, T properties)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, PropertiesToDictionary(properties)));
        }

        // TODO(krait): resolve conflicts
        public static void Info(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, PropertiesToDictionary(parameters)));
        }

        private static IReadOnlyDictionary<string, object> PropertiesToDictionary<T>(T properties)
        {
            throw new NotImplementedException();
        }

        public static LogEvent SetProperty<T>(this LogEvent @event, string key, T value)
        {
            return new LogEvent(@event.Level, @event.Timestamp, @event.MessageTemplate, @event.Properties.Concat(new[] { new KeyValuePair<string, object>(key, value), }).ToDictionary(p => p.Key, p => p.Value), @event.Exception);
        }
    }
}