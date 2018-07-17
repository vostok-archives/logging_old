using Vostok.Logging.Core;
using Vostok.Logging.FileLog.Configuration;

namespace Vostok.Logging.FileLog
{
    internal static class FileLogSettingsValidatorExtensions
    {
        public static SettingsValidationResult Validate(this FileLogSettings settings)
        {
            return validator.TryValidate(settings);
        }

        private static readonly FileLogSettingsValidator validator = new FileLogSettingsValidator();
    }
}