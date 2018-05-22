using System;
using System.IO;
using System.Linq;
using Vostok.Logging.Configuration.Settings;

namespace Vostok.Logging.Extensions
{
    internal static class SettingsValidator
    {
        public static SettingsValidationResult Validate<TSettings>(this TSettings settings)
        {
            if (typeof (TSettings) == typeof (FileLogSettings))
                return Validate(settings as FileLogSettings);

            if (typeof(TSettings) == typeof(ConsoleLogSettings))
                return Validate(settings as ConsoleLogSettings);

            return SettingsValidationResult.NotSupportedSettingsType(typeof(TSettings));
        }

        public static SettingsValidationResult Validate(this FileLogSettings settings)
        {
            if (settings?.Encoding == null)
                return SettingsValidationResult.EncodingIsNull();

            if (settings.ConversionPattern == null)
                return SettingsValidationResult.ConversionPatternIsNull();

            return FilePathIsValid(settings.FilePath);
        }

        public static SettingsValidationResult Validate(this ConsoleLogSettings settings)
        {
            if (settings?.ConversionPattern == null)
                return SettingsValidationResult.ConversionPatternIsNull();

            return SettingsValidationResult.Success();
        }

        private static SettingsValidationResult FilePathIsValid(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return SettingsValidationResult.FilePathIsNullOrEmpty();

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(filePath);
            }
            catch (NotSupportedException exception)
            {
                return SettingsValidationResult.FilePathIsNotCorrect(filePath, exception);
            }

            var directoryName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryName))
                return SettingsValidationResult.DirectoryNotFound(directoryName);

            return SettingsValidationResult.Success();
        }
    }

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
                return type.ToString();

            var propertiesStr = string.Join(",", properties.GetType().GetProperties().Select(p => $"{p.Name}={p.GetValue(properties)}"));
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