using System;
using System.Collections.Generic;
using System.Text;

namespace Vostok.Logging
{
    public  static class LogEventFormatter
    {
        public static string Format(LogEvent @event)
        {
            var message = FormatMessage(@event.MessageTemplate, @event.Properties);
            return $"{@event.Timestamp:HH:mm:ss.fff} {@event.Level} {message} {@event.Exception}{Environment.NewLine}";
        }

        public static string FormatMessage(string messageTemplate, IReadOnlyDictionary<string, object> properties)
        {
            if (messageTemplate == null || properties == null)
                return null;

            var message = new StringBuilder();
            for (var i = 0; i < messageTemplate.Length; i++)
            {
                var currentChar = messageTemplate[i];
                if (!currentChar.Equals('{'))
                    message.Append(currentChar);
                else
                {
                    if (!TryGetTokenFrom(messageTemplate, i, out var token))
                    {
                        if (token != null)
                        {
                            message.Append(token);
                        }
                    }
                    else
                    {
                        var propertyName = token.ToString(1, token.Length - 2);
                        if (!string.IsNullOrWhiteSpace(propertyName) && properties.ContainsKey(propertyName))
                        {
                            var property = properties[propertyName];
                            message.Append(property);
                        }
                        else
                        {
                            message.Append(token);
                        }
                    }

                    i += token?.Length - 1 ?? 0;
                }
            }
            return message.ToString();
        }

        public static bool TryGetTokenFrom(string messageTemplate, int startIndex, out StringBuilder token)
        {
            if (startIndex < 0 || startIndex > messageTemplate.Length - 1)
            {
                token = null;
                return false;
            }

            token = new StringBuilder().Append(messageTemplate[startIndex]);

            if (!token[0].Equals('{'))
                return false;

            for (var i = startIndex + 1; i < messageTemplate.Length; i++)
            {
                var currentChar = messageTemplate[i];
                if (currentChar.Equals('{'))
                    return false;
                token.Append(currentChar);
                if (currentChar.Equals('}'))
                    return true;
            }

            return false;
        }
    }
}
