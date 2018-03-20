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
            //log.Log(new LogEvent(LogLevel.Info, message, null, DateTime.UtcNow));
        }

        // TODO(krait): Extension with params
        public static void Info<T>(this ILog log, string messageTemplate, T properties)
        {
            if(!typeof(T).IsConstructedGenericType)
                return;

            //if (log.IsEnabledFor(LogLevel.Info))
                //log.Log(new LogEvent(LogLevel.Info, messageTemplate, PropertiesToDictionary(properties), DateTime.UtcNow));
        }

        // TODO(krait): resolve conflicts
        public static void Info(this ILog log, string messageTemplate, params object[] parameters)
        {
            //log.Log(new LogEvent(LogLevel.Info, messageTemplate, PropertiesToDictionary(parameters), DateTime.UtcNow));
        }

        private static IDictionary<string, object> PropertiesToDictionary<T>(T properties)
        {
            throw new NotImplementedException();
        }

        public static LogEvent SetProperty<T>(this LogEvent @event, string key, T value)
        {
            return null;
            //return new LogEvent(@event.Level, @event.MessageTemplate, @event.Properties.Concat(new[] { new KeyValuePair<string, object>(key, value), }).ToDictionary(p => p.Key, p => p.Value), @event.Timestamp);
        }
    }
}