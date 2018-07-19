namespace Vostok.Logging.Core
{
    internal static class Console
    {
        public static void TryWriteLine(string message)
        {
            try
            {
                System.Console.Out.WriteLine(message);
            }
            catch
            {
                // ignored
            }
        }

        public static void TryWriteLine(object obj)
        {
            TryWriteLine(obj?.ToString());
        }

        public static void TryWrite(string message)
        {
            try
            {
                System.Console.Out.Write(message);
            }
            catch
            {
                // ignored
            }
        }

        public static void TryWrite(object obj)
        {
            TryWrite(obj?.ToString());
        }
    }
}