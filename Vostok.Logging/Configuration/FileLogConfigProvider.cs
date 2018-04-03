using System;
using System.Configuration;
using System.Text;
using Vostok.Commons;

namespace Vostok.Logging.Configuration
{
    internal class FileLogConfigProvider : IFileLogConfigProvider
    {
        public string FilePath => GetSettingOrDefault(Section.FilePath.Value, "C:\\logs\\log");
        public bool ImmediateFlush => GetSettingOrDefault(Section.ImmediateFlush.Value, true);
        public bool AppendToFile => GetSettingOrDefault(Section.AppendToFile.Value, true);
        public string ConversionPattern => GetSettingOrDefault(Section.ConversionPattern.Value, "");
        public DataSize MaxFileSize => GetSettingOrDefault(Section.MaxFileSize.Value, DataSize.FromMegabytes(5));
        public RollingStyle RollingStyle => GetSettingOrDefault(Section.RollingStyle.Value, RollingStyle.Date);
        public string DatePattern => GetSettingOrDefault(Section.DatePattern.Value, "yyyy.MM.dd");
        public Encoding Encoding => GetSettingOrDefault(Section.Encoding.Value, Encoding.Default);

        private static FileLogConfigSection Section => (FileLogConfigSection)ConfigurationManager.GetSection(FileLogConfigSectionName);

        private static string GetSettingOrDefault(string value, string defaultValue)
        {
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        private static bool GetSettingOrDefault(string value, bool defaultValue)
        {
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        private static DataSize GetSettingOrDefault(string value, DataSize defaultValue)
        {
            return DataSize.TryParse(value, out var result) ? result : defaultValue;
        }

        private static RollingStyle GetSettingOrDefault(string value, RollingStyle defaultValue)
        {
            return Enum.TryParse(value, out RollingStyle result) ? result : defaultValue;
        }

        private static Encoding GetSettingOrDefault(string value, Encoding defaultValue)
        {
            try
            {
                return Encoding.GetEncoding(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private const string FileLogConfigSectionName = "fileLogConfig";
    }
}