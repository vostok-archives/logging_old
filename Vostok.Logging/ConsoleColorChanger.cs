using System;

namespace Vostok.Logging
{
    internal class ConsoleColorChanger : IDisposable
    {
        public ConsoleColorChanger(ConsoleColor newColor)
        {
            oldColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
        }

        public void Dispose()
        {
            Console.ForegroundColor = oldColor;
        }

        private readonly ConsoleColor oldColor;
    }
}