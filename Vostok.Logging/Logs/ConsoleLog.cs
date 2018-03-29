using System;
using System.Collections.Generic;
using System.Threading;
using Vostok.Commons.ThreadManagment;

namespace Vostok.Logging.Logs
{
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
            StartNewLoggingThread();
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            eventsBuffer.TryAdd(@event);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        private static void StartNewLoggingThread()
        {
            ThreadRunner.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            WriteEventsToConsole();
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(300);
                        }
                        if (eventsBuffer.Count == 0)
                            eventsBuffer.WaitForNewItems();
                    }
                });
        }

        private static void WriteEventsToConsole()
        {
            var eventsCount = eventsBuffer.Drain(currentEvents, 0, currentEvents.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentEvents[i];
                using (new ConsoleColorChanger(levelToColor[currentEvent.Level]))
                {
                    Console.Out.Write(FormatEvent(currentEvent));
                }
            }
        }

        private static string FormatEvent(LogEvent @event)
        {
            var message = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);
            return $"{@event.Timestamp:HH:mm:ss.fff} {@event.Level} {message} {@event.Exception}{Environment.NewLine}";
        }

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private const int Capacity = 10000;
    }
}