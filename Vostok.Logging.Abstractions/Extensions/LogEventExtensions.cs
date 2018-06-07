using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Logging.Abstractions
{
    public static class LogEventExtensions
    {
        public static LogEvent WithProperty<T>(this LogEvent @event, string key, T value)
        {
            Dictionary<string, object> properties;

            if (@event.Properties != null)
            {
                if (@event.Properties.ContainsKey(key))
                {
                    properties = @event.Properties.ToDictionary(p => p.Key, p => p.Value, StringComparer.CurrentCultureIgnoreCase);
                    properties[key] = value;
                }
                else
                {
                    properties = @event.Properties
                        .Concat(new []{ new KeyValuePair<string, object>(key, value) })
                        .ToDictionary(p => p.Key, p => p.Value, StringComparer.CurrentCultureIgnoreCase);
                }
            }
            else
            {
                properties = new Dictionary<string, object>{{key, value}};
            }

            return new LogEvent(@event.Level, @event.Timestamp, @event.MessageTemplate, properties, @event.Exception);
        }
    }
}