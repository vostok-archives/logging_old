using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal class ConsoleLogSettingsValidator : ILogSettingsValidator<ConsoleLogSettings>
    {
        public SettingsValidationResult Validate(ConsoleLogSettings settings)
        {
            if (settings?.ConversionPattern == null)
                return SettingsValidationResult.ConversionPatternIsNull();

            return SettingsValidationResult.Success();
        }
    }
}