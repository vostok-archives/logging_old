using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Conversions;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
using Vostok.Logging.Core.Configuration;

namespace Vostok.Logging.FileLog
{
    public class FileLog : ILog
    {
        static FileLog()
        {
            configProvider = new LogConfigProvider<FileLogSettings>(configSectionName);
        }

        public static void Configure(FileLogSettings settings)
        {
            configProvider = new LogConfigProvider<FileLogSettings>(settings);
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if(!IsInitialized)
                Initialize();

            eventsBuffer.TryAdd(@event);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        private static void StartNewLoggingThread()
        {
            Task.Run(async () =>
            {
                var startDate = DateTimeOffset.UtcNow.Date;
                var settings = configProvider.Settings;
                var writer = CreateFileWriter(settings);

                while (true)
                {
                    var settingsWereUpdated = TryUpdateSettings(ref settings);

                    try
                    {
                        var currentDate = DateTimeOffset.UtcNow.Date;

                        if (settingsWereUpdated || settings.EnableRolling && currentDate > startDate)
                        {
                            writer.Close();
                            writer = CreateFileWriter(settings);
                            startDate = currentDate;
                        }

                        WriteEventsToFile(writer, settings);
                    }
                    catch (Exception exception)
                    {
                        Console.Out.WriteLine(exception);
                        await Task.Delay(300.Milliseconds());
                    }

                    if (eventsBuffer.Count == 0)
                    {
                        eventsBuffer.WaitForNewItems();
                    }
                }
            });
        }

        private static bool TryUpdateSettings(ref FileLogSettings settings)
        {
            var newSettings = configProvider.Settings;

            if (newSettings.Equals(settings))
                return false;

            settings = newSettings;
            return true;
        }

        private static TextWriter CreateFileWriter(FileLogSettings settings)
        {
            var fileName = settings.EnableRolling 
                ? $"{settings.FilePath}{DateTimeOffset.UtcNow.Date:yyyy.MM.dd}" 
                : settings.FilePath;

            var fileMode = settings.AppendToFile 
                ? FileMode.Append 
                : FileMode.OpenOrCreate;

            var file = File.Open(fileName, fileMode, FileAccess.Write, FileShare.Read);
            return new StreamWriter(file, settings.Encoding);
        }

        private static void WriteEventsToFile(TextWriter writer, FileLogSettings settings)
        {
            var buffer = eventsBuffer;
            var eventsToWrite = currentEventsBuffer;

            if (settings.EventsQueueCapacity != capacity)
                ReinitEventsQueue(settings);

            var eventsCount = buffer.Drain(eventsToWrite, 0, eventsToWrite.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = eventsToWrite[i];
                writer.Write(settings.ConversionPattern.Format(currentEvent));
            }
            writer.Flush();
        }

        private static void Initialize()
        {
            if (Interlocked.CompareExchange(ref isInitializedFlag, 1, 0) != isInitializedFlag)
            {
                ReinitEventsQueue(configProvider.Settings);
                StartNewLoggingThread();
            }
        }

        private static void ReinitEventsQueue(FileLogSettings settings)
        {
            capacity = settings.EventsQueueCapacity;
            currentEventsBuffer = new LogEvent[capacity];
            eventsBuffer = new BoundedBuffer<LogEvent>(capacity);
        }

        private static bool IsInitialized => isInitializedFlag == 1;

        private static int isInitializedFlag;

        private static ILogConfigProvider<FileLogSettings> configProvider;

        private static int capacity;
        private static LogEvent[] currentEventsBuffer;
        private static BoundedBuffer<LogEvent> eventsBuffer;

        private const string configSectionName = "fileLogConfig";
    }
}