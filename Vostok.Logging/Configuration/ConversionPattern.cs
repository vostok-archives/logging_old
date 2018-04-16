using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vostok.Logging.Configuration
{
    public class ConversionPattern
    {
        public string PatternStr { get; }

        public static ConversionPattern FromString(string patternStr)
        {
            return new ConversionPattern(patternStr, true);
        }

        public static bool TryParse(string patternStr, out ConversionPattern result)
        {
            result = null;
            if (patternStr == null)
                return false;

            result = FromString(patternStr);
            return true;
        }

        public static ConversionPattern Default => new ConversionPattern(string.Join(" ", patternKeys.Keys.Select(k => $"{{{k}}}")), false);

        public string Format(LogEvent @event)
        {
            var timestamp = @event.Timestamp.ToString("HH:mm:ss zzz");
            var level = @event.Level;
            var message = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);
            var exception = @event.Exception;
            var newLine = Environment.NewLine;

            var properties = @event.Properties != null 
                ? string.Join(", ", @event.Properties.Values
                    .Select(p => (p as IFormattable)?.ToString(null, CultureInfo.InvariantCulture) ?? p.ToString())) 
                : null;

            var formattedLine = string.Format(formatString, timestamp, level, message, exception, newLine, properties);

            if (@event.Properties == null)
                return formattedLine;

            return Regex.Replace(formattedLine, SinglePropertyPattern, 
                m => @event.Properties.TryGetValue(m.Groups[1].Value, out var value)
                    ? value.ToString() 
                    : m.Value);
        }

        private ConversionPattern(string patternStr, bool replaceKeys)
        {
            PatternStr = patternStr;
            patternStr = string.IsNullOrEmpty(patternStr)
                ? string.Empty
                : patternStr;

            formatString = patternStr;

            if (replaceKeys)
            {
                foreach (var key in patternKeys.Keys)
                {
                    var value = patternKeys[key];
                    formatString = Regex.Replace(formatString, value, _ => $"{{{key}}}");
                }

                formatString = Regex.Replace(formatString, AllPropertiesPattern, _ => $"{{{patternKeys.Count}}}");
            }
        }

        public override int GetHashCode()
        {
            return formatString.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConversionPattern);
        }

        private bool Equals(ConversionPattern other)
        {
            if (other == null)
                return false;

            return formatString.Equals(other.formatString, StringComparison.CurrentCultureIgnoreCase);
        }

        private readonly string formatString;

        private static readonly Dictionary<int, string> patternKeys = new Dictionary<int, string>
        {
            {0, "%(?:d|D)" },
            {1, "%(?:l|L)" },
            {2, "%(?:m|M)" },
            {3, "%(?:e|E)" },
            {4, "%(?:n|N)" }
        };

        private const string SinglePropertyPattern = "%(?:p|P)\\((\\w*)\\)";
        private const string AllPropertiesPattern = "%(?:p|P)(?!\\()";
    }
}