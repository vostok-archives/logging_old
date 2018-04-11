using System;
using System.Text;

namespace Vostok.Logging.Configuration.Parsing
{
    internal static class EncodingParser
    {
        public static bool TryParse(string value, out Encoding result)
        {
            result = null;
            try
            {
                result = Encoding.GetEncoding(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}