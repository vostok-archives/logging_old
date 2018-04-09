using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using Vostok.Commons.Conversions;
using Vostok.Commons.ThreadManagment;

namespace Vostok.Logging.Configuration.FileLog
{
    internal class FileLogConfigProvider : IFileLogConfigProvider
    {
        public FileLogSettings Settings { get; private set; }

        public FileLogConfigProvider()
        {
            Settings = CreateNewSettings();
            ThreadRunner.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        UpdateCache();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    Thread.Sleep(updateCachePeriod);
                }
            });
        }

        private void UpdateCache()
        {
            var section = Section;
            if(section == null)
                return;

            var settingsWereChanged = TryUpdateFilePath(section) ||
                                      TryUpdateConversionPattern(section) ||
                                      TryUpdateAppendToFile(section) ||
                                      TryUpdateEnableRolling(section) ||
                                      TryUpdateEncoding(section);

            if(!settingsWereChanged)
                return;

            Settings = CreateNewSettings();
        }

        private FileLogSettings CreateNewSettings()
        {
            return new FileLogSettings
            {
                FilePath = filePath,
                ConversionPattern = conversionPattern,
                AppendToFile = appendToFile,
                EnableRolling = enableRolling,
                Encoding = encoding
            };
        }

        private bool TryUpdateFilePath(FileLogConfigSection section)
        {
            var value = section.FilePath.Value;
            if (filePath.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                return false;

            if (!Directory.Exists(Path.GetDirectoryName(value)))
                return false;

            //TODO(mylov): Need for check of file name correctness
            filePath = value;
            return true;
        }

        private bool TryUpdateConversionPattern(FileLogConfigSection section)
        {
            var value = section.ConversionPattern.Value;
            if (conversionPattern.PatternStr.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                return false;

            conversionPattern = ConversionPattern.FromString(value);
            return true;
        }

        private bool TryUpdateAppendToFile(FileLogConfigSection section)
        {
            if (bool.TryParse(section.AppendToFile.Value, out var value))
            {
                appendToFile = value;
                return true;
            }

            return false;
        }

        private bool TryUpdateEnableRolling(FileLogConfigSection section)
        {
            if (bool.TryParse(section.EnableRolling.Value, out var value))
            {
                enableRolling = value;
                return true;
            }

            return false;
        }

        private bool TryUpdateEncoding(FileLogConfigSection section)
        {
            try
            {
                encoding = Encoding.GetEncoding(section.Encoding.Value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static FileLogConfigSection Section => (FileLogConfigSection)ConfigurationManager.GetSection(FileLogConfigSectionName);

        private string filePath = "C:\\logs\\log";
        private ConversionPattern conversionPattern = ConversionPattern.Default;
        private bool appendToFile = true;
        private bool enableRolling = true;
        private Encoding encoding = Encoding.Default;

        private readonly TimeSpan updateCachePeriod = 5.Seconds();

        private const string FileLogConfigSectionName = "fileLogConfig";
    }
}