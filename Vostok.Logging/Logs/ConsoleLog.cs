using System;
using System.Collections.Generic;

namespace Vostok.Logging.Logs
{
    // CR(krait): This one should become asynchronous.
    public class ConsoleLog : ILog
    {
        public void Log(LogEvent @event)
        {
            lock (syncLock)
            {
                using (new ConsoleColorChanger(levelToColor[@event.Level]))
                {
                    Console.Out.Write(LogEventFormatter.Format(@event));
                }
            }
        }

        public bool IsEnabledFor(LogLevel level)
        {
            throw new System.NotImplementedException();
        }

        private static readonly object syncLock = new object();

        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };
    }
}