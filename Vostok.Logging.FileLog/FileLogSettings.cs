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
    }
}