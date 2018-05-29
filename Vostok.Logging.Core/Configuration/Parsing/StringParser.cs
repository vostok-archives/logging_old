namespace Vostok.Logging.Core.Configuration.Parsing
{
    internal static class StringParser
    {
        public static bool TryParse(string value, out string result)
        {
            result = null;
            if (value == null)
                return false;

            result = value;
            return true;
        }
    }
}