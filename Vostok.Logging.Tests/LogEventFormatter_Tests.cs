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
        public void TryGetTokenFrom_should_return_false_for_negative_index_value()
        {
            LogEventFormatter.TryGetTokenFrom(Template, -1, out var token).Should().BeFalse();
            token.Should().BeNull();
        }

        [Test]
        public void TryGetTokenFrom_should_return_false_for_very_large_index_value()
        {
            LogEventFormatter.TryGetTokenFrom(Template, 100, out var token).Should().BeFalse();
            token.Should().BeNull();
        }

        [Test]
        public void TryGetTokenFrom_should_return_false_if_first_symbol_is_not_left_brace()
        {
            LogEventFormatter.TryGetTokenFrom(Template, 0, out var token).Should().BeFalse();
            token.ToString().Should().BeEquivalentTo("a");
        }

        [Test]
        public void TryGetTokenFrom_should_return_true_if_correct_token_was_found()
        {
            LogEventFormatter.TryGetTokenFrom(Template, 1, out var token).Should().BeTrue();
            token.ToString().Should().BeEquivalentTo("{a}");
        }

        [Test]
        public void TryGetTokenFrom_should_return_false_if_second_left_brace_was_found()
        {
            LogEventFormatter.TryGetTokenFrom(Template, 5, out var token).Should().BeFalse();
            token.ToString().Should().BeEquivalentTo("{a");
        }

        [Test]
        public void TryGetTokenFrom_should_return_false_if_right_brace_was_not_found()
        {
            LogEventFormatter.TryGetTokenFrom(Template, 7, out var token).Should().BeFalse();
            token.ToString().Should().BeEquivalentTo("{a");
        }

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

        [Test]
        public void FormatMessage_should_not_replace_placeholder_for_empty_property_name()
        {
            var properties = new Dictionary<string, object> { { "", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithEmptyProp, properties).Should().BeEquivalentTo("ab{cd{}r}gt}tr{gty{");
        }

        [Test]
        public void FormatMessage_should_not_replace_placeholder_for_whitespace_property_name()
        {
            var properties = new Dictionary<string, object> { { " ", "hello" } };
            LogEventFormatter.FormatMessage(TemplateWithWhitespaceProp, properties).Should().BeEquivalentTo("ab{cd{ }r}gt}tr{gty{");
        }

        [Test]
        public void FormatMessage_should_return_null_for_null_template()
        {
            var properties = new Dictionary<string, object> { { " ", "hello" } };
            LogEventFormatter.FormatMessage(null, properties).Should().BeNull();
        }

        [Test]
        public void FormatMessage_should_return_null_for_null_properties()
        {
            LogEventFormatter.FormatMessage(TemplateWithProp, null).Should().BeNull();
        }

        private const string Template = "a{a}a{a{a";
        private const string TemplateWithProp = "ab{cd{prop}r}gt}tr{gty{";
        private const string TemplateWithEmptyProp = "ab{cd{}r}gt}tr{gty{";
        private const string TemplateWithWhitespaceProp = "ab{cd{ }r}gt}tr{gty{";
    }
}
