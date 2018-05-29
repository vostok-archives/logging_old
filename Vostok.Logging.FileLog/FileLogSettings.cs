using System;
using System.Text;
using Vostok.Logging.Core.Configuration;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Vostok.Logging.FileLog
{
    public class FileLogSettings
    {
        public string FilePath { get; set; } = "C:\\logs\\log";
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;
        public bool AppendToFile { get; set; } = true;
        public bool EnableRolling { get; set; } = true;
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public override int GetHashCode()
        {
            return (FilePath?.ToLower().GetHashCode() ?? 0) * 42 + 
                   (ConversionPattern?.GetHashCode() ?? 0) * 276 + 
                   (AppendToFile ? 1 : 0) * 967 + 
                   (EnableRolling ? 1 : 0) * 352 + 
                   Encoding.GetHashCode() * 475;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileLogSettings);
        }

        private bool Equals(FileLogSettings other)
        {
            if (other == null)
                return false;

            var filePathesAreEqual = FilePath == null && other.FilePath == null || 
                                     FilePath != null && FilePath.Equals(other.FilePath, StringComparison.CurrentCultureIgnoreCase);

            var conversionPatternsAreEqual = ConversionPattern == null && other.ConversionPattern == null || 
                                             ConversionPattern != null && ConversionPattern.Equals(other.ConversionPattern);

            var encodingsAreEqual = Encoding == null && other.Encoding == null ||
                                    Encoding != null && Encoding.Equals(other.Encoding);


            return filePathesAreEqual &&
                   conversionPatternsAreEqual && 
                   AppendToFile == other.AppendToFile && 
                   EnableRolling == other.EnableRolling && 
                   encodingsAreEqual;
        }
    }
}