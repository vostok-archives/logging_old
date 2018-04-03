using System.Configuration;

namespace Vostok.Logging.Configuration
{
    internal class FileLogConfigSection : ConfigurationSection
    {
        [ConfigurationProperty(FilePathPropertyName)]
        public FileLogConfigElement FilePath => (FileLogConfigElement)base[FilePathPropertyName];

        [ConfigurationProperty(ImmediateFlushPropertyName)]
        public FileLogConfigElement ImmediateFlush => (FileLogConfigElement)base[ImmediateFlushPropertyName];

        [ConfigurationProperty(AppendToFilePropertyName)]
        public FileLogConfigElement AppendToFile => (FileLogConfigElement)base[AppendToFilePropertyName];

        [ConfigurationProperty(ConversionPatternPropertyName)]
        public FileLogConfigElement ConversionPattern => (FileLogConfigElement)base[ConversionPatternPropertyName];

        [ConfigurationProperty(MaxFileSizePropertyName)]
        public FileLogConfigElement MaxFileSize => (FileLogConfigElement)base[MaxFileSizePropertyName];

        [ConfigurationProperty(RollingStylePropertyName)]
        public FileLogConfigElement RollingStyle => (FileLogConfigElement)base[RollingStylePropertyName];

        [ConfigurationProperty(DatePatternPropertyName)]
        public FileLogConfigElement DatePattern => (FileLogConfigElement)base[DatePatternPropertyName];

        [ConfigurationProperty(EncodingPropertyName)]
        public FileLogConfigElement Encoding => (FileLogConfigElement)base[EncodingPropertyName];

        private const string FilePathPropertyName = "file";
        private const string ImmediateFlushPropertyName = "immediateFlush";
        private const string AppendToFilePropertyName = "appendToFile";
        private const string ConversionPatternPropertyName = "conversionPattern";
        private const string MaxFileSizePropertyName = "maximumFileSize";
        private const string RollingStylePropertyName = "rollingStyle";
        private const string DatePatternPropertyName = "datePattern";
        private const string EncodingPropertyName = "encoding";
    }
}