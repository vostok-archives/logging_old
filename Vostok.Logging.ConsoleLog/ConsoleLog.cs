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
    public class ConsoleLog : ILog
    {
        static ConsoleLog()
        {
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
                        Console.Out.WriteLine(exception);
                        await Task.Delay(300.Milliseconds());
                    }

                    if (eventsBuffer.Count == 0)
                    {
                        // CR(krait): Nope, we must wait asynchronously here. Now we still waste a thread while waiting.
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

        // CR(krait): Switch to AtomicBoolean.
        private static bool IsInitialized => isInitializedFlag == 1;

        private static int isInitializedFlag;

        private static ILogConfigProvider<ConsoleLogSettings> configProvider;

        private static int capacity; // CR(krait): This field can be replaced with currentEventsBuffer.Length
        private static volatile LogEvent[] currentEventsBuffer;
        private static volatile BoundedBuffer<LogEvent> eventsBuffer;

        private const string configSectionName = "consoleLogConfig";
    }
}