using System;
using System.Collections.Generic;
using System.Threading;

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
            if(@event == null)
                return;

            eventsQueue.Enqueue(@event);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        private static void StartNewLoggingThread()
        {
            var thread = new Thread(() =>
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
                    }
                });
            thread.Start();
        }

        private static void WriteEventsToConsole()
        {
            for (var i = 0; i < EventsToWriteForIteration; i++)
            {
                if (!eventsQueue.TryDequeue(out var currentEvent))
                {
                    break;
                }

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

        private static readonly CycledQueue<LogEvent> eventsQueue = new CycledQueue<LogEvent>(QueueCapacity);

        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private const int QueueCapacity = 10000;
        private const int EventsToWriteForIteration = 1000;
    }
}