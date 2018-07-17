using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Core
{
    public class ConversionPattern
    {
        static ConversionPattern()
        {
            patternKeys = new Dictionary<PatternPartType, string>
            {
                { PatternPartType.DateTime, "d" },
                { PatternPartType.Level, "l" },
                { PatternPartType.Prefix, "x" },
                { PatternPartType.Message, "m" },
                { PatternPartType.Exception, "e" },
                { PatternPartType.Property, @"p\((\w*)\)" },
                { PatternPartType.Properties, "p" },
                { PatternPartType.NewLine, "n" }
            };

            var anyKeyRegex = string.Join("|", patternKeys.Values);

            regexPattern = $"(?:^?%(?:{anyKeyRegex})|^)((?:[^%]|%(?!{anyKeyRegex}))*)";

            Default = new ConversionPattern("%d %l %x %m %e %n");
        }

        public static ConversionPattern Default { get; }

        public string PatternStr { get; }

        public static ConversionPattern FromString([CanBeNull] string patternStr)
        {
            return new ConversionPattern(patternStr);
        }

        public static bool TryParse([CanBeNull] string patternStr, out ConversionPattern result)
        {
            result = null;
            if (patternStr == null)
                return false;

            result = FromString(patternStr);
            return true;
        }

        public string Format([NotNull] LogEvent @event)
        {
            if (patternParts.Length == 0)
                return string.Empty;

            var builder = patternParts[0].Type == PatternPartType.StringStart
                ? new StringBuilder(patternParts[0].PartSuffix, patternParts.Length * 10)
                : new StringBuilder(patternParts.Length * 10);

            foreach (var part in patternParts.Where(p => p.Type != PatternPartType.StringStart))
            {
                var partValue = GetPartValue(part, @event);

                if (part.Type == PatternPartType.Message)
                    partValue = LogEventFormatter.FormatMessage(partValue, @event.Properties);

                if (!string.IsNullOrEmpty(partValue))
                {
                    builder.Append(partValue);
                    builder.Append(part.PartSuffix);
                }
            }

            return builder.ToString();
        }

        private static string GetPartValue(PatternPart part, LogEvent @event)
        {
            switch (part.Type)
            {
                case PatternPartType.DateTime:
                    return @event.Timestamp.ToString(dateTimeFormatString);

                case PatternPartType.Level:
                    return @event.Level.ToString();

                case PatternPartType.Prefix:
                    var prefixProperty = GetPropertyOrNull(@event, prefixPropertyName);
                    return prefixProperty is IReadOnlyList<string> prefixes
                        ? string.Join(" ", prefixes.Select(p => $"[{TryFormatProperty(p)}]"))
                        : null;

                case PatternPartType.Message:
                    return @event.MessageTemplate;

                case PatternPartType.Exception:
                    return @event.Exception?.ToString();

                case PatternPartType.Property:
                    var property = GetPropertyOrNull(@event, part.PropertyName);
                    return TryFormatProperty(property);

                case PatternPartType.Properties:
                    return TryFormatProperties(@event.Properties);

                case PatternPartType.NewLine:
                    return Environment.NewLine;

                default:
                    return null;
            }
        }

        private static object GetPropertyOrNull(LogEvent @event, string propertyName)
        {
            if (@event.Properties == null)
                return null;

            return @event.Properties.TryGetValue(propertyName, out var property)
                ? property
                : null;
        }

        private static string TryFormatProperty(object property)
        {
            if (property == null)
                return null;

            return (property as IFormattable)?.ToString(null, CultureInfo.CurrentCulture) ?? property.ToString();
        }

        private static string TryFormatProperties(IReadOnlyDictionary<string, object> properties)
        {
            if (properties == null)
                return null;

            return $"[properties: {string.Join(", ", properties.Select(p => $"{p.Key} = {TryFormatProperty(p.Value)}"))}]";
        }

        private ConversionPattern(string patternStr)
        {
            if (string.IsNullOrEmpty(patternStr))
            {
                PatternStr = string.Empty;
                patternParts = new PatternPart[0];
                return;
            }

            PatternStr = patternStr;

            var matches = Regex.Matches(patternStr, regexPattern, RegexOptions.IgnoreCase);
            patternParts = new PatternPart[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                patternParts[i] = CreatePatternPart(match);
            }
        }

        private static PatternPart CreatePatternPart(Match match)
        {
            var value = match.Groups[0].Value;
            var propertyName = match.Groups[1].Value;
            var suffix = match.Groups[2].Value;

            if (!value.StartsWith("%"))
                return new PatternPart(PatternPartType.StringStart, null, suffix);

            foreach (var patternType in patternKeys.Keys.Where(k => k != PatternPartType.Property))
            {
                var patternKey = $"%{patternKeys[patternType]}";
                if (value.StartsWith(patternKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(propertyName))
                        return new PatternPart(PatternPartType.Property, propertyName, suffix);

                    return new PatternPart(patternType, null, suffix);
                }
            }

            return new PatternPart(PatternPartType.StringStart, null, value);
        }

        public override int GetHashCode()
        {
            return PatternStr.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConversionPattern);
        }

        private bool Equals(ConversionPattern other)
        {
            if (other == null)
                return false;

            return PatternStr.Equals(other.PatternStr, StringComparison.CurrentCultureIgnoreCase);
        }

        private static readonly Dictionary<PatternPartType, string> patternKeys;
        private static readonly string regexPattern;

        private readonly PatternPart[] patternParts;

        private const string dateTimeFormatString = "HH:mm:ss zzz";
        private const string prefixPropertyName = "prefix";

        private enum PatternPartType
        {
            StringStart,
            DateTime,
            Level,
            Prefix,
            Message,
            Exception,
            Properties,
            Property,
            NewLine
        }

        private class PatternPart
        {
            public PatternPartType Type { get; }
            public string PropertyName { get; }
            public string PartSuffix { get; }

            public PatternPart(PatternPartType type, string propertyName, string suffix)
            {
                Type = type;
                PropertyName = propertyName;
                PartSuffix = suffix;
            }
        }
    }
}