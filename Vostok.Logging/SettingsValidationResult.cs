using System;
using System.Linq;

namespace Vostok.Logging
{
    internal class SettingsValidationResult
    {
        public bool IsSuccessful => type == ValidationResultType.Success;

        public static SettingsValidationResult NotSupportedSettingsType(Type settingsType)
        {
            return new SettingsValidationResult(ValidationResultType.NotSupportedSettingsType, new
            {
                SettingsType = settingsType
            });
        }
        public static SettingsValidationResult EncodingIsNull()
        {
            return new SettingsValidationResult(ValidationResultType.EncodingIsNull);
        }
        public static SettingsValidationResult ConversionPatternIsNull()
        {
            return new SettingsValidationResult(ValidationResultType.ConversionPatternIsNull);
        }

        public static SettingsValidationResult FilePathIsNullOrEmpty()
        {
            return new SettingsValidationResult(ValidationResultType.FilePathIsNullOrEmpty);
        }

        public static SettingsValidationResult FilePathIsNotCorrect(string filePath, Exception exception)
        {
            return new SettingsValidationResult(ValidationResultType.FilePathIsNotCorrect, new
            {
                FilePath = filePath,
                Exception = exception
            });
        }

        public static SettingsValidationResult DirectoryNotFound(string directoryPath)
        {
            return new SettingsValidationResult(ValidationResultType.DirectoryNotFound, new
            {
                DirectoryPath = directoryPath
            });
        }

        public static SettingsValidationResult Success()
        {
            return new SettingsValidationResult(ValidationResultType.Success);
        }

        private SettingsValidationResult(ValidationResultType type, object properties = null)
        {
            this.type = type;
            this.properties = properties;
        }

        public void EnsureSuccess()
        {
            if (IsSuccessful)
                return;

            throw new SettingsValidationFailedException(this);
        }

        public override string ToString()
        {
            if (properties == null || properties.GetType().GetProperties().Length == 0)
                return $"{type}";

            var propertiesStr = string.Join(", ", properties.GetType().GetProperties().Select(p => $"{p.Name}={p.GetValue(properties)}"));
            return $"{type}: {{{propertiesStr}}}";

        }

        private readonly ValidationResultType type;
        private readonly object properties;

        private enum ValidationResultType
        {
            NotSupportedSettingsType,
            EncodingIsNull,
            ConversionPatternIsNull,
            FilePathIsNullOrEmpty,
            FilePathIsNotCorrect,
            DirectoryNotFound,
            Success
        }

        private class SettingsValidationFailedException : Exception
        {
            public SettingsValidationFailedException(SettingsValidationResult result) : base($"Settings validation failed: {{{result}}}") { }
        }
    }
}