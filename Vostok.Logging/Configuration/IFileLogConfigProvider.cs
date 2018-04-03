using System.Text;
using Vostok.Commons;

namespace Vostok.Logging.Configuration
{
    internal interface IFileLogConfigProvider
    {
        string FilePath { get; }
        bool ImmediateFlush { get; }
        bool AppendToFile { get; }
        string ConversionPattern { get; }
        DataSize MaxFileSize { get; }
        RollingStyle RollingStyle { get; }
        string DatePattern { get; }
        Encoding Encoding { get; }
    }
}