using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Commons.Conversions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.ConsoleLog
{
    // CR(krait): Most of the comments also apply to FileLog.
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
            Task.Run(() =>
                {
                    // CR(krait): This delay looks very suspicious. What is it for?
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
            // CR(krait): Let's not waste a thread for this. We can run it on the thread pool and use `await Task.Delay` to sleep, thus releasing the thread when unused. FIXED
            Task.Run(async () => 
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
                        Console.Out.WriteLine(exception); // CR(krait): Why is it Console.WriteLine here and Console.Out.WriteLine there? FIXED
                        await Task.Delay(300.Milliseconds());
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

        private const int Capacity = 10000; // CR(krait): This should be configured (and be warm).
        private const string ConfigSectionName = "consoleLogConfig";
    }
}