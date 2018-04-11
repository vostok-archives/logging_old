using Vostok.Logging.Configuration.Settings;

namespace Vostok.Logging.Extensions
{
    internal static class SettingsValidator
    {
        public static bool AreValid<TSettings>(this TSettings settings)
        {
            if (typeof (TSettings) == typeof (FileLogSettings))
                return AreValid(settings as FileLogSettings);

            if (typeof(TSettings) == typeof(ConsoleLogSettings))
                return AreValid(settings as ConsoleLogSettings);

            return false;
        }

        public static bool AreValid(this FileLogSettings settings)
        {
            if (settings?.Encoding == null || settings.ConversionPattern == null)
                return false;

            return FilePathIsValid(settings.FilePath);
        }

        public static bool AreValid(this ConsoleLogSettings settings)
        {
            if (settings?.ConversionPattern == null)
                return false;

            return true;
        }

        private static bool FilePathIsValid(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            //TODO(mylov) Add filePath validation
            return true;
        }
    }
}
