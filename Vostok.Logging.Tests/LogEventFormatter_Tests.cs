using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class LogEventFormatter_Tests
    {
        [Test]
        public void FormatMessage_should_replace_placeholder_if_needed_property_exists()
        {
            var properties = new Dictionary<string, object> {{"prop", "hello"}};
            LogEventFormatter.FormatMessage(TemplateWithProp, properties).Should().BeEquivalentTo("ab{cdhellor}gt}tr{gty{");
        }

        [Test]
        public void FormatMessage_should_not_replace_placeholder_if_properties_are_not_case_ignored()
        {
            var properties = new Dictionary<string, object> { { "ProP", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithProp, properties).Should().BeEquivalentTo("ab{cd{prop}r}gt}tr{gty{");
        }

        [Test]
        public void FormatMessage_should_replace_placeholder_if_properties_are_case_ignored()
        {
            var properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase) { { "ProP", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithProp, properties).Should().BeEquivalentTo("ab{cdhellor}gt}tr{gty{");
        }

        [Test]
        public void FormatMessage_should_not_replace_placeholder_if_needed_property_is_absent()
        {
            var properties = new Dictionary<string, object> { { "otherProp", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithProp, properties).Should().BeEquivalentTo("ab{cd{prop}r}gt}tr{gty{");
        }

        // CR(krait): Why?
        [Test]
        public void FormatMessage_should_not_replace_placeholder_for_empty_property_name()
        {
            var properties = new Dictionary<string, object> { { "", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithEmptyProp, properties).Should().BeEquivalentTo("ab{cd{}r}gt}tr{gty{");
        }

        // CR(krait): Why?
        [Test]
        public void FormatMessage_should_not_replace_placeholder_for_whitespace_property_name()
        {
            var properties = new Dictionary<string, object> { { " ", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithWhitespaceProp, properties).Should().BeEquivalentTo("ab{cd{ }r}gt}tr{gty{");
        }

        // CR(krait): Why? It seems sane to throw an ArgumentNullException.
        [Test]
        public void FormatMessage_should_return_null_for_null_template()
        {
            var properties = new Dictionary<string, object> { { " ", "hello" } };
            LogEventFormatter.FormatMessage(null, properties).Should().BeNull();
        }

        // CR(krait): Why? It seems sane to throw an ArgumentNullException.
        [Test]
        public void FormatMessage_should_return_null_for_null_properties()
        {
            LogEventFormatter.FormatMessage(TemplateWithProp, null).Should().BeNull();
        }

        // CR(krait): Need tests for more corner cases.

        // CR(krait): Need tests for { and } escaping. E.g. that "{{0}}" is not substituted. 

        private const string Template = "a{a}a{a{a";
        private const string TemplateWithProp = "ab{cd{prop}r}gt}tr{gty{";
        private const string TemplateWithEmptyProp = "ab{cd{}r}gt}tr{gty{";
        private const string TemplateWithWhitespaceProp = "ab{cd{ }r}gt}tr{gty{";


        private const string Template2 = "{0}_{1}"; //veryveryveryverybig_string
    }
}
