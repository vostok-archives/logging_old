﻿namespace Vostok.Logging.FileLog
{
    internal static class FileLogSettingsValidatorExtensions
    {
        public static SettingsValidationResult Validate(this FileLogSettings settings)
        {
            return validator.Validate(settings);
        }

        private static readonly FileLogSettingsValidator validator = new FileLogSettingsValidator();
    }
}