using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Logging.Configuration
{
    internal class ConversionPattern
    {
        public string PatternStr { get; }

        public static ConversionPattern FromString(string patternStr)
        {
            return new ConversionPattern(patternStr, true);
        }

        public static ConversionPattern Default => new ConversionPattern(string.Join(" ", patternKeys.Keys.Select(k => $"{{{k}}}")), false);

        private ConversionPattern(string patternStr, bool replaceKeys)
        {
            PatternStr = patternStr;
            formatString = patternStr;

            if (replaceKeys)
            {
                foreach (var key in patternKeys.Keys)
                {
                    var value = patternKeys[key];
                    //TODO(mylov): Do something with ignorecasing
                    formatString = formatString.Replace(value, $"{{{key}}}");
                }
            }
        }

        public string Format(LogEvent @event)
        {
            var timestamp = @event.Timestamp.ToString("HH:mm:ss zzz");
            var level = @event.Level;
            var message = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);
            var exception = @event.Exception;
            var newLine = Environment.NewLine;
            return string.Format(formatString, timestamp, level, message, exception, newLine);
        }

        private readonly string formatString;

        private static readonly Dictionary<int, string> patternKeys = new Dictionary<int, string>
        {
            {0, "-d" },
            {1, "-l" },
            {2, "-m" },
            {3, "-e" },
            {4, "-n" }
        };
    }
}