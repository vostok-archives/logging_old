// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Vostok.Logging.Configuration.Settings
{
    public class ConsoleLogSettings
    {
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;

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