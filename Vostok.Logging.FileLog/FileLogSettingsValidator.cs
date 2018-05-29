using System;
using System.IO;
using Vostok.Logging.Core;

namespace Vostok.Logging.FileLog
{
    internal class FileLogSettingsValidator : ILogSettingsValidator<FileLogSettings>
    {
        public SettingsValidationResult Validate(FileLogSettings settings)
        {
            if (settings?.Encoding == null)
                return SettingsValidationResult.EncodingIsNull();

            if (settings.ConversionPattern == null)
                return SettingsValidationResult.ConversionPatternIsNull();

            return FilePathIsValid(settings.FilePath);
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
}