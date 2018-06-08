using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// The formatter used to fill log message templates with property and parameter values.
    /// </summary>
    public static class LogEventFormatter
    {
        /// <summary>
        /// <para>Substitutes placeholders in <paramref name="template"/> with the actual property values from the given dictionary.</para>
        /// <para>The dictionary contains both named properties and unnamed parameters.</para>
        /// <list type="bullet">
        ///     <listheader>Substitution rules:</listheader>
        ///     <item>{name} will be replaced with the value of property 'name'.</item>
        ///     <item>{0} will be replaced with the first of the unnamed parameters.</item>
        ///     <item>Use {{ and }} to escape curly braces in <paramref name="template"/>.</item>
        ///     <item>Placeholders for nonexistent properties and invalid placeholders are not replaced.</item>
        /// </list>
        /// </summary>
        public static string FormatMessage(string template, IReadOnlyDictionary<string, object> properties)
        {
            if (template == null)
                return null;

            if (properties == null)
                return template;

            var resultBuilder = new StringBuilder(template.Length * 3);
            var tokenBuilder = new TokenBuilder(template.Length);

            for (var i = 0; i < template.Length; i++)
            {
                var currentChar = template[i];

                if (currentChar != '{' && currentChar != '}')
                {
                    tokenBuilder.Add(currentChar);
                    continue;
                }

                if (!tokenBuilder.IsEmpty)
                    tokenBuilder.MoveToBuilder(resultBuilder);

                if (i == template.Length - 1)
                {
                    tokenBuilder.Add(currentChar);
                    continue;
                }

                var nextChar = template[i + 1];
                if (currentChar == nextChar)
                {
                    tokenBuilder.Add(currentChar);
                    i++;
                    continue;
                }

                if (currentChar == '}')
                {
                    tokenBuilder.Add(currentChar);
                    continue;
                }

                var findTokenResult = tokenBuilder.TryFindToken(template, i);

                i += tokenBuilder.Length - 1;

                if (findTokenResult)
                {
                    var key = tokenBuilder.GetKeyFromBuffer();
                    if (properties.TryGetValue(key, out var value))
                    {
                        resultBuilder.Append((value as IFormattable)?.ToString(null, CultureInfo.InvariantCulture) ?? value);
                        tokenBuilder.Clear();
                    }
                }
            }

            if (!tokenBuilder.IsEmpty)
                tokenBuilder.MoveToBuilder(resultBuilder);

            return resultBuilder.ToString();
        }

        private struct TokenBuilder
        {
            public int Length { get; private set; }

            public TokenBuilder(int length)
            {
                Length = 0;
                chars = new char[length];
            }

            public bool TryFindToken(string template, int startIndex)
            {
                if (startIndex < 0 || startIndex > template.Length - 1)
                    return false;

                var currentChar = template[startIndex];
                Add(currentChar);

                if (currentChar != '{')
                    return false;

                for (var i = startIndex + 1; i < template.Length; i++)
                {
                    currentChar = template[i];

                    if (currentChar == '{')
                        return false;

                    Add(currentChar);

                    if (currentChar == '}')
                        return true;
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void MoveToBuilder(StringBuilder builder)
            {
                if (Length == 0)
                    return;

                builder.Append(chars, 0, Length);
                Length = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string GetKeyFromBuffer()
            {
                return Length <= 2
                    ? string.Empty
                    : new string(chars, 1, Length - 2);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(char c)
            {
                chars[Length++] = c;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear()
            {
                Length = 0;
            }

            public bool IsEmpty
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return Length == 0; }
            }

            private readonly char[] chars;
        }
    }
}