using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Vostok.Commons;
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


        private static string GetComponentValue(EventComponentsType type, LogEvent @event, string propertyName)
        {
            switch (type)
            {
                case EventComponentsType.DateTime:
                    return @event.Timestamp.ToString("HH:mm:ss zzz");

                case EventComponentsType.Level:
                    return @event.Level.ToString();

                case EventComponentsType.Prefix:
                    var prefixProperty = GetPropertyOrNull(@event, prefixPropertyName);
                    return prefixProperty is IReadOnlyList<string> prefixes ? string.Join(" ", prefixes.Select(p => $"[{p.ToString()}]")): null;

                case EventComponentsType.Message:
                    return @event.MessageTemplate;

                case EventComponentsType.Exception:
                    return @event.Exception?.ToString();

                case EventComponentsType.Property:
                    return GetPropertyOrNull(@event, propertyName)?.ToString();

                case EventComponentsType.Properties:
                    return @event.Properties == null ? null : $"[properties: {string.Join(", ", @event.Properties.Select(p => $"{p.Key} = {p.Value.ToString()}"))}]";

                case EventComponentsType.NewLine:
                    return Environment.NewLine;

                default:
                    return null;
            }
        }

        public string Format([NotNull] LogEvent @event)
        {
            var builder = new StringBuilder(parts[0].type == EventComponentsType.StringStart ? parts[0].partSuffix : null);

            for (var i = 0; i < parts.Length; i++)
            {
                var componentType = parts[i].type;
                var postStr = parts[i].partSuffix;
                var value = GetComponentValue(componentType, @event, parts[i].propertyName);
                if (!string.IsNullOrEmpty(value))
                {
                    builder.Append(value);
                    builder.Append(postStr);
                }
            }

            return builder.ToString();
        }

        private ConversionPattern(string patternStr)
        {
            var anyKey = string.Join("|", patternKeys.Values);
            var matches = Regex.Matches(patternStr, $"(?:^?%(?:{anyKey})|^)((?:[^%]|%(?!{anyKey}))*)", RegexOptions.IgnoreCase);
            parts = new (EventComponentsType type, string propertyName, string partSuffix)[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                parts[i] = TransformMatch(match);
            }
        }

        private (EventComponentsType type, string propertyName, string partSuffix) TransformMatch(Match match)
        {
            var value = match.Value;
            var propertyName = match.Groups[1].Value;
            var suffix = match.Groups[2].Value;

            if (!value.StartsWith("%"))
                return (EventComponentsType.StringStart, null, suffix);

            foreach (var key in patternKeys.Keys.Where(k => k != EventComponentsType.Property))
            {
                var pattern = $"%{patternKeys[key]}";
                if (value.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(propertyName))
                        return (EventComponentsType.Property, propertyName, suffix);

                    return (key, null, suffix);
                }
            }

            return (EventComponentsType.StringStart, null, value);
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

        private readonly (EventComponentsType type, string propertyName, string partSuffix)[] parts;

        private static readonly Dictionary<EventComponentsType, string> patternKeys = new Dictionary<EventComponentsType, string>
        {
            { EventComponentsType.DateTime, "d" },
            { EventComponentsType.Level, "l" },
            { EventComponentsType.Prefix, "x" },
            { EventComponentsType.Message, "m" },
            { EventComponentsType.Exception, "e" },
            { EventComponentsType.Property, @"p\((\w*)\)" },
            { EventComponentsType.Properties, "p" },
            { EventComponentsType.NewLine, "n" }
        };

        private const string prefixPropertyName = "prefix";

        private enum EventComponentsType
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
    }
}