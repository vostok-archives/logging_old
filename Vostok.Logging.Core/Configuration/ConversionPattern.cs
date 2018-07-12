using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Core.Configuration
{
    public class ConversionPattern
    {
        public string PatternStr { get; }

        public static ConversionPattern FromString([CanBeNull] string patternStr)
        {
            return new ConversionPattern(patternStr, true);
        }

        public static bool TryParse([CanBeNull] string patternStr, out ConversionPattern result)
        {
            result = null;
            if (patternStr == null)
                return false;

            result = FromString(patternStr);
            return true;
        }

        public static ConversionPattern Default => new ConversionPattern(string.Join(string.Empty, patternKeys.Keys.Select(k => $"{{{k}}}")), false);

        public string Format([NotNull] LogEvent @event)
        {
            object prefixProperty = null;
            @event.Properties?.TryGetValue(prefixPropertyName, out prefixProperty);
            var prefixes = prefixProperty is ImmutableArray<string> array && !array.IsDefaultOrEmpty ? array.ToArray() : null;
            var message = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);
            var properties = @event.Properties != null
                ? string.Join(", ", @event.Properties.Values
                    .Select(p => (p as IFormattable)?.ToString(null, CultureInfo.InvariantCulture) ?? p.ToString()))
                : null;

            var timestampStr = @event.Timestamp.ToString("HH:mm:ss zzz");
            var levelStr = @event.Level;
            var prefixStr = prefixes == null ? null : string.Join(" ", prefixes.Select(p => $"[{p}]"));
            var messageStr = string.IsNullOrEmpty(message) ? null : message;
            var exceptionStr = @event.Exception;
            var newLine = Environment.NewLine;
            var propertiesStr = string.IsNullOrEmpty(properties) ? null : properties;

            var formattedLine = string.Format(formatString, timestampStr, levelStr, prefixStr, messageStr, exceptionStr, newLine, propertiesStr);

            if (@event.Properties == null)
                return formattedLine;

            return Regex.Replace(formattedLine, singlePropertyPattern, 
                m => @event.Properties.TryGetValue(m.Groups[1].Value, out var value)
                    ? value.ToString() 
                    : m.Value);
        }

        private ConversionPattern([CanBeNull] string patternStr, bool replaceKeys)
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

                formatString = Regex.Replace(formatString, allPropertiesPattern, _ => $"{{{patternKeys.Count}}}");
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
            {2, "%(?:x|X)" },
            {3, "%(?:m|M)" },
            {4, "%(?:e|E)" },
            {5, "%(?:n|N)" }
        };

        private const string singlePropertyPattern = "%(?:p|P)\\((\\w*)\\)";
        private const string allPropertiesPattern = "%(?:p|P)(?!\\()";
        private const string prefixPropertyName = "prefix";
    }
}