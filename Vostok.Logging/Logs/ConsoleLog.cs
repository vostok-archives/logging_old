using System;

namespace Vostok.Logging.Logs
{
    public class ConsoleLog : ILog
    {
        public void Log(LogEvent @event)
        {
            lock (syncLock)
            {
                var oldColor = Console.ForegroundColor;
                //Console.ForegroundColor = levelToColor[logEvent.Level];
                //Console.Out.Write(LogEventFormatter.Format(logEvent));
                //Console.ForegroundColor = oldColor;
            }
        }

        public bool IsEnabledFor(LogLevel level)
        {
            throw new System.NotImplementedException();
        }

        private static readonly object syncLock = new object();
    }
}