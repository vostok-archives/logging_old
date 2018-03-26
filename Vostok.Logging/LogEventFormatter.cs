using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vostok.Logging
{
    public static class LogEventFormatter
    {
        // TODO(krait): Probably contains bugs. More tests are required.
        public static string FormatMessage(string template, IReadOnlyDictionary<string, object> properties)
        {
            if (template == null || properties == null)
                return null;

            var partBuffer = new StringBuffer(template.Length);
            var resultBuilder = new StringBuilder(template.Length * 3);

            var inKey = false;
            for (var i = 0; i < template.Length; i++)
            {
                var currentChar = template[i];
                if (currentChar == '{')
                {
                    if (inKey)
                        resultBuilder.Append('{');
                    if (inKey && partBuffer.IsEmpty)
                    {
                        resultBuilder.Append('{');
                        inKey = false;
                    }
                    else
                    {
                        inKey = true;
                        partBuffer.MoveToBuilder(resultBuilder);
                    }
                    continue;
                }

                if (currentChar == '}' && inKey)
                {
                    var key = partBuffer.MoveToString();
                    if (properties.TryGetValue(key, out var value))
                        resultBuilder.Append(value);
                    else
                        resultBuilder.Append('{').Append(key).Append('}');

                    inKey = false;
                    continue;
                }

                partBuffer.Add(currentChar);
            }

            if (inKey)
                resultBuilder.Append('{');
            if (!partBuffer.IsEmpty)
                partBuffer.MoveToBuilder(resultBuilder);

            return resultBuilder.ToString();
        }

        private struct StringBuffer
        {
            public StringBuffer(int length)
            {
                index = 0;
                chars = new char[length];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void MoveToBuilder(StringBuilder builder)
            {
                if (index == 0)
                    return;

                builder.Append(chars, 0, index);
                index = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string MoveToString()
            {
                if (index == 0)
                    return "";

                var value = new string(chars, 0, index);
                index = 0;
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(char c)
            {
                chars[index++] = c;
            }

            public bool IsEmpty
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return index == 0; }
            }

            private int index;
            private readonly char[] chars;
        }
    }
}
