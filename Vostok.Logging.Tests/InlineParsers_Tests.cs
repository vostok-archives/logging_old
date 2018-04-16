using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration;
using Vostok.Logging.Configuration.Parsing;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class InlineParsers_Tests
    {
        [TestCase(typeof(string))]
        [TestCase(typeof(ConversionPattern))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(Encoding))]
        public void TryParse_should_return_correct_results_for_supported_InlineParsers(Type parserType)
        {
            var inlineParser = typeToParserTest[parserType].parser;
            var scenarios = typeToParserTest[parserType].scenarios;

            foreach (var (input, boolResult, valueResult) in scenarios)
            {
                inlineParser.TryParse(input, out var result).Should().Be(boolResult);
                result.Should().Be(valueResult);
            }
        }

        private readonly Dictionary<Type, (IInlineParser parser, (string input, bool boolResult, object valueResult)[] scenarios)> 
            typeToParserTest  = new Dictionary<Type, (IInlineParser, (string, bool, object)[])>
        {
            {typeof(string), (new InlineParser<string>(StringParser.TryParse), new (string, bool, object)[]
            {
                (null, false, null),
                (string.Empty, true, string.Empty),
                ("Hello, World", true, "Hello, World")
            })},
            {typeof(ConversionPattern), (new InlineParser<ConversionPattern>(ConversionPattern.TryParse), new (string, bool, object)[]
            {
                (null, false, null),
                (string.Empty, true, ConversionPattern.FromString(string.Empty)),
                ("Hello, World", true, ConversionPattern.FromString("Hello, World")),
                ("%d %m %p %l", true, ConversionPattern.FromString("{0} {2} {5} {1}"))
            })},
            {typeof(bool), (new InlineParser<bool>(bool.TryParse), new (string, bool, object)[]
            {
                (null, false, null),
                (string.Empty, false, null),
                ("Hello, World", false, null),
                ("true", true, true),
                ("True", true, true),
                ("false", true, false),
                ("False", true, false)
            })},
            {typeof(Encoding), (new InlineParser<Encoding>(EncodingParser.TryParse), new (string, bool, object)[]
            {
                (null, false, null),
                (string.Empty, false, null),
                ("Hello, World", false, null),
                ("utf-8", true, Encoding.UTF8)
            })}
        };
    }
}