using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.ConsoleLog
{
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
            Task.Run(() =>
                {
                    Task.Delay(100).GetAwaiter().GetResult();
                    configProvider = configProvider ?? new LogConfigProvider<ConsoleLogSettings>(ConfigSectionName, new ConsoleLogSettingsValidator());
                    StartNewLoggingThread();
                });
        }

        public static void Configure(Func<ConsoleLogSettings> settingsSource)
        {
            configProvider?.Dispose();
            configProvider = new LogConfigProvider<ConsoleLogSettings>(settingsSource, new ConsoleLogSettingsValidator());
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
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
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