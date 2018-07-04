using System;
using System.Text;
using Vostok.Configuration.Abstractions;
using Vostok.Logging.Core.Configuration;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Vostok.Logging.FileLog
{
    [ValidateBy(typeof(FileLogSettingsValidator))]
    public class FileLogSettings
    {
        public string FilePath { get; set; } = "C:\\logs\\log"; // CR(krait): A bad default.
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;
        public bool AppendToFile { get; set; } = true;
        public bool EnableRolling { get; set; } = true;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public int EventsQueueCapacity { get; set; } = 10000;

        //// CR(krait): Do you actually need to compare FileLogSettings?
        //public override int GetHashCode()
        //{
        //    return (FilePath?.ToLower().GetHashCode() ?? 0) * 23 + 
        //           (ConversionPattern?.GetHashCode() ?? 0) * 17 + 
        //           (AppendToFile ? 1 : 0) * 29 + 
        //           (EnableRolling ? 1 : 0) * 13 + 
        //           Encoding.GetHashCode() * 97 + 
        //           EventsQueueCapacity * 3;
        //}

        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as FileLogSettings);
        //}

        //private bool Equals(FileLogSettings other)
        //{
        //    if (other == null)
        //        return false;

        //    var filePathesAreEqual = FilePath == null && other.FilePath == null ||
        //                             FilePath != null && FilePath.Equals(other.FilePath, StringComparison.CurrentCultureIgnoreCase);

        //    var conversionPatternsAreEqual = ConversionPattern == null && other.ConversionPattern == null ||
        //                                     ConversionPattern != null && ConversionPattern.Equals(other.ConversionPattern);

        //    var encodingsAreEqual = Encoding == null && other.Encoding == null ||
        //                            Encoding != null && Encoding.Equals(other.Encoding);


        //    return filePathesAreEqual &&
        //           conversionPatternsAreEqual &&
        //           AppendToFile == other.AppendToFile &&
        //           EnableRolling == other.EnableRolling &&
        //           encodingsAreEqual &&
        //           EventsQueueCapacity == other.EventsQueueCapacity;
        //}
    }
}