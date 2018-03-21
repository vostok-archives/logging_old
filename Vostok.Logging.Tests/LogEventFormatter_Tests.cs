using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class LogEventFormatter_Tests
    {
        private const string Template = "a{a}a{a{a";

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
        public void TryGetTokenFrom_should_return_true_if_first_symbol_is_correct_token_was_found()
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
    }
}
