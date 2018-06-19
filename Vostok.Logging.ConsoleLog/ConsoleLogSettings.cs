// ReSharper disable NonReadonlyMemberInGetHashCode
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.ConsoleLog
{
    public class ConsoleLogSettings
    {
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;

        // CR(Mansiper): multiplication is unnecessary. Why not just return ConversionPattern?.GetHashCode() ?? 0;
        public override int GetHashCode()
        {
            return (ConversionPattern?.GetHashCode() ?? 0) * 276;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConsoleLogSettings);
        }

        private bool Equals(ConsoleLogSettings other)
        {
            if (other == null)
                return false;

            return ConversionPattern == null && other.ConversionPattern == null ||
                   ConversionPattern != null && ConversionPattern.Equals(other.ConversionPattern);
        }
    }
}