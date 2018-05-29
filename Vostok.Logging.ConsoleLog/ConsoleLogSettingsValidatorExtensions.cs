using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogSettingsValidatorExtensions
    {
        public static SettingsValidationResult Validate(this ConsoleLogSettings settings)
        {
            return validator.Validate(settings);
        }

        private static readonly ConsoleLogSettingsValidator validator = new ConsoleLogSettingsValidator();
    }
}