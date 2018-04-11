using System.Text;

namespace Vostok.Logging.Configuration.Settings
{
    public class FileLogSettings
    {
        public string FilePath { get; set; } = "C:\\logs\\log";
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;
        public bool AppendToFile { get; set; } = true;
        public bool EnableRolling { get; set; } = true;
        public Encoding Encoding { get; set; } = Encoding.Default;
    }
}