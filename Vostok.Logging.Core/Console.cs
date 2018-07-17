using System;

namespace Vostok.Logging.Core
{
    internal static class Console
    {
        public static void TryOutToConsole(string message, bool writeLine = false)
        {
            try
            {
                if (writeLine)
                    System.Console.Out.WriteLine(message);
                else
                    System.Console.Out.Write(message);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void TryOutToConsole(Exception exception, bool writeLine = false)
        {
            TryOutToConsole(exception.ToString(), writeLine);
        }
    }
}