using System.Configuration;

namespace Vostok.Logging.Configuration
{
    internal class FileLogConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(FilePathPropertyName)]
        public FileLogConfigElement FilePath => (FileLogConfigElement)base[FilePathPropertyName];

        [ConfigurationProperty(AppendToFilePropertyName)]
        public FileLogConfigElement AppendToFile => (FileLogConfigElement)base[AppendToFilePropertyName];

        [ConfigurationProperty(ConversionPatternPropertyName)]
        public FileLogConfigElement ConversionPattern => (FileLogConfigElement)base[ConversionPatternPropertyName];

        [ConfigurationProperty(EnableRollingPropertyName)]
        public FileLogConfigElement EnableRolling => (FileLogConfigElement)base[EnableRollingPropertyName];

        [ConfigurationProperty(EncodingPropertyName)]
        public FileLogConfigElement Encoding => (FileLogConfigElement)base[EncodingPropertyName];

        private const string FilePathPropertyName = "file";
        private const string AppendToFilePropertyName = "appendToFile";
        private const string ConversionPatternPropertyName = "conversionPattern";
        private const string EnableRollingPropertyName = "enableRolling";
        private const string EncodingPropertyName = "encoding";
    }
}