using System;
using System.Collections.Generic;
using System.Threading;

namespace Vostok.Logging.Logs
{
    // CR(krait): This one should become asynchronous.
    public class ConsoleLog : ILog
    {
        public void Log(LogEvent @event)
        {
            if(@event == null)
                return;

            eventsQueue.Enqueue(@event);
        }

        public bool IsEnabledFor(LogLevel level) => true;

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

        private static Thread StartNewLoggingThread()
        {
            var thread = new Thread(WriteEventsToConsole);
            thread.Start();
            return thread;
        }

        private static readonly Thread logThread = StartNewLoggingThread();

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