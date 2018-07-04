using System;
using System.IO;
using System.Threading.Tasks;
using Vostok.Commons.Conversions;
using Vostok.Commons.Synchronization;
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
            isInitialized = new AtomicBoolean(false);
        }

        public static void Configure(FileLogSettings settings)
        {
            configProvider = new LogConfigProvider<FileLogSettings>(settings);
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if(!isInitialized)
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
                        //await eventsBuffer.WaitForNewItemsAsync();
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

            var directory = Path.GetDirectoryName(fileName);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var file = File.Open(fileName, fileMode, FileAccess.Write, FileShare.Read);
            return new StreamWriter(file, settings.Encoding);
        }

        private static void WriteEventsToFile(TextWriter writer, FileLogSettings settings)
        {
            var buffer = eventsBuffer;
            var eventsToWrite = currentEventsBuffer;

            if (settings.EventsQueueCapacity != currentEventsBuffer.Length)
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
            if (isInitialized.TrySetTrue())
            {
                ReinitEventsQueue(configProvider.Settings);
                StartNewLoggingThread();
            }
        }

        private static void ReinitEventsQueue(FileLogSettings settings)
        {
            currentEventsBuffer = new LogEvent[settings.EventsQueueCapacity];
            eventsBuffer = new BoundedBuffer<LogEvent>(settings.EventsQueueCapacity);
        }

        private static readonly AtomicBoolean isInitialized;

        private static ILogConfigProvider<FileLogSettings> configProvider;

        private static LogEvent[] currentEventsBuffer;
        private static BoundedBuffer<LogEvent> eventsBuffer;

        private const string configSectionName = "fileLogConfig";
    }
}