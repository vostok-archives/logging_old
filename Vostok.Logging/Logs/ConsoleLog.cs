using System;
using System.Collections.Generic;
using System.Threading;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration;
using Vostok.Logging.Configuration.Settings;

namespace Vostok.Logging.Logs
{
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
            configProvider = new LogConfigProvider<ConsoleLogSettings>(ConfigSectionName);
            StartNewLoggingThread();
        }

        public void Configure(Func<ConsoleLogSettings> settingsSource)
        {
            configProvider = new LogConfigProvider<ConsoleLogSettings>(settingsSource);
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
                    var settings = configProvider.Settings;
                    try
                    {
                        WriteEventsToConsole(settings);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(300);
                    }

                    if (eventsBuffer.Count == 0)
                    {
                        eventsBuffer.WaitForNewItems();
                    }
                }
            });
        }

        private static void WriteEventsToConsole(ConsoleLogSettings settings)
        {
            var eventsCount = eventsBuffer.Drain(currentEvents, 0, currentEvents.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentEvents[i];
                using (new ConsoleColorChanger(levelToColor[currentEvent.Level]))
                {
                    Console.Out.Write(settings.ConversionPattern.Format(currentEvent));
                }
            }
        }

        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private static ILogConfigProvider<ConsoleLogSettings> configProvider;

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private const int Capacity = 10000;
        private const string ConfigSectionName = "consoleLogConfig";
    }
}