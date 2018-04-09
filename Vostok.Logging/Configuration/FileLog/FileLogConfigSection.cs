using System.Configuration;

namespace Vostok.Logging.Configuration.FileLog
{
    internal class FileLogConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(FilePathPropertyName)]
        public LogConfigElement FilePath => (LogConfigElement)base[FilePathPropertyName];

        [ConfigurationProperty(AppendToFilePropertyName)]
        public LogConfigElement AppendToFile => (LogConfigElement)base[AppendToFilePropertyName];

        [ConfigurationProperty(ConversionPatternPropertyName)]
        public LogConfigElement ConversionPattern => (LogConfigElement)base[ConversionPatternPropertyName];

        [ConfigurationProperty(EnableRollingPropertyName)]
        public LogConfigElement EnableRolling => (LogConfigElement)base[EnableRollingPropertyName];

        [ConfigurationProperty(EncodingPropertyName)]
        public LogConfigElement Encoding => (LogConfigElement)base[EncodingPropertyName];

        private const string FilePathPropertyName = "file";
        private const string AppendToFilePropertyName = "appendToFile";
        private const string ConversionPatternPropertyName = "conversionPattern";
        private const string EnableRollingPropertyName = "enableRolling";
        private const string EncodingPropertyName = "encoding";
    }
}