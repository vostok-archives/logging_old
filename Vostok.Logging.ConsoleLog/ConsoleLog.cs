using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Conversions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.ConsoleLog
{
    // CR(krait): Most of the comments also apply to FileLog. FIXED
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
            // CR(krait): This delay looks very suspicious. What is it for? FIXED
            configProvider = new LogConfigProvider<ConsoleLogSettings>(configSectionName);
        }

        public static void Configure(ConsoleLogSettings settings)
        {
            configProvider = new LogConfigProvider<ConsoleLogSettings>(settings);
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!IsInitialized)
                Initialize();

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
            var buffer = eventsBuffer;
            var eventsToWrite = currentEventsBuffer;

            if (settings.EventsQueueCapacity != capacity)
                ReinitEventsQueue(settings);

            var eventsCount = buffer.Drain(eventsToWrite, 0, eventsToWrite.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = eventsToWrite[i];
                using (new ConsoleColorChanger(levelToColor[currentEvent.Level]))
                {
                    Console.Out.Write(settings.ConversionPattern.Format(currentEvent));
                }
            }
        }

        private static void Initialize()
        {
            if (Interlocked.CompareExchange(ref isInitializedFlag, 1, 0) != isInitializedFlag)
            {
                ReinitEventsQueue(configProvider.Settings);
                StartNewLoggingThread();
            }
        }

        private static void ReinitEventsQueue(ConsoleLogSettings settings)
        {
            capacity = settings.EventsQueueCapacity;
            currentEventsBuffer = new LogEvent[capacity];
            eventsBuffer = new BoundedBuffer<LogEvent>(capacity);
        }

        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private static bool IsInitialized => isInitializedFlag == 1;

        private static int isInitializedFlag;

        private static ILogConfigProvider<ConsoleLogSettings> configProvider;

        private static int capacity; // CR(krait): This should be configured (and be warm). FIXED
        private static LogEvent[] currentEventsBuffer;
        private static BoundedBuffer<LogEvent> eventsBuffer;

        private const string configSectionName = "consoleLogConfig";
    }
}