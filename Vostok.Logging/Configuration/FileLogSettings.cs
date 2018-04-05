using System.Text;

namespace Vostok.Logging.Configuration
{
    internal class FileLogSettings
    {
        public string FilePath { get; set; }
        public ConversionPattern ConversionPattern { get; set; }
        public bool AppendToFile { get; set; }
        public bool EnableRolling { get; set; }
        public Encoding Encoding { get; set; }
    }
}