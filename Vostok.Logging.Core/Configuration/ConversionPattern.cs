using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Core.Configuration
{
    public class ConversionPattern
    {
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

        public static ConversionPattern Default = new ConversionPattern(string.Join(string.Empty, patternKeys.Keys.Select(k => $"{{{k}}}")));

        private static string GetComponentValue(TemplateComponentPart part, LogEvent @event)
        {
            switch (part.Type)
            {
                case FormatPatternType.DateTime:
                    return @event.Timestamp.ToString("HH:mm:ss zzz");

                case FormatPatternType.Level:
                    return @event.Level.ToString();

                case FormatPatternType.Prefix:
                    var prefixProperty = GetPropertyOrNull(@event, prefixPropertyName);
                    return prefixProperty is IReadOnlyList<string> prefixes ? string.Join(" ", prefixes.Select(p => $"[{p.ToString()}]")): null;

                case FormatPatternType.Message:
                    return @event.MessageTemplate;

                case FormatPatternType.Exception:
                    return @event.Exception?.ToString();

                case FormatPatternType.Property:
                    return GetPropertyOrNull(@event, part.PropertyName)?.ToString();

                case FormatPatternType.Properties:
                    return @event.Properties == null ? null : $"[properties: {string.Join(", ", @event.Properties.Select(p => $"{p.Key} = {p.Value.ToString()}"))}]";

                case FormatPatternType.NewLine:
                    return Environment.NewLine;

                default:
                    return null;
            }
        }

        public string Format([NotNull] LogEvent @event)
        {
            var builder = new StringBuilder(parts[0].Type == FormatPatternType.StringStart ? parts[0].PartSuffix : null);

            foreach (var part in parts)
            {
                var value = GetComponentValue(part, @event);
                if (!string.IsNullOrEmpty(value))
                {
                    builder.Append(value);
                    builder.Append(part.PartSuffix);
                }
            }

            return builder.ToString();
        }

        private ConversionPattern(string patternStr)
        {
            var anyKey = string.Join("|", patternKeys.Values);
            var matches = Regex.Matches(patternStr, $"(?:^?%(?:{anyKey})|^)((?:[^%]|%(?!{anyKey}))*)", RegexOptions.IgnoreCase);
            parts = new TemplateComponentPart[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                parts[i] = TransformMatch(match);
            }
        }

        private TemplateComponentPart TransformMatch(Match match)
        {
            var value = match.Value;
            var propertyName = match.Groups[1].Value;
            var suffix = match.Groups[2].Value;

            if (!value.StartsWith("%"))
                return new TemplateComponentPart(FormatPatternType.StringStart, null, suffix);

            foreach (var key in patternKeys.Keys.Where(k => k != FormatPatternType.Property))
            {
                var pattern = $"%{patternKeys[key]}";
                if (value.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(propertyName))
                        return new TemplateComponentPart(FormatPatternType.Property, propertyName, suffix);

                    return new TemplateComponentPart(key, null, suffix);
                }
            }

            return new TemplateComponentPart(FormatPatternType.StringStart, null, value);
        }

        public override int GetHashCode()
        {
            return 0;//formatString.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConversionPattern);
        }

        private bool Equals(ConversionPattern other)
        {
            if (other == null)
                return false;

            return true;//formatString.Equals(other.formatString, StringComparison.CurrentCultureIgnoreCase);
        }

        private static object GetPropertyOrNull(LogEvent @event, string propertyName)
        {
            if (@event.Properties == null)
                return null;

            return @event.Properties.TryGetValue(propertyName, out var property)
                ? property
                : null;
        }

        private readonly TemplateComponentPart[] parts;

        private static readonly Dictionary<FormatPatternType, string> patternKeys = new Dictionary<FormatPatternType, string>
        {
            { FormatPatternType.DateTime, "d" },
            { FormatPatternType.Level, "l" },
            { FormatPatternType.Prefix, "x" },
            { FormatPatternType.Message, "m" },
            { FormatPatternType.Exception, "e" },
            { FormatPatternType.Property, @"p\((\w*)\)" },
            { FormatPatternType.Properties, "p" },
            { FormatPatternType.NewLine, "n" }
        };

        private const string prefixPropertyName = "prefix";

        private enum FormatPatternType
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

        private class TemplateComponentPart
        {
            public FormatPatternType Type { get; }
            public string PropertyName { get; }
            public string PartSuffix { get; }

            public TemplateComponentPart(FormatPatternType type, string propertyName, string suffix)
            {
                Type = type;
                PropertyName = propertyName;
                PartSuffix = suffix;
            }
        }
    }
}