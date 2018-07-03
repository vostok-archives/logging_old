using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogSettingsValidatorExtensions
    {
        public static SettingsValidationResult Validate(this ConsoleLogSettings settings)
        {
            return validator.TryValidate(settings);
        }

        private static readonly ConsoleLogSettingsValidator validator = new ConsoleLogSettingsValidator();
    }
}