using System;
using System.IO;
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
            Task.Run(() =>
            {
                //Task.Delay(100).GetAwaiter().GetResult();
                configProvider = configProvider ?? new LogConfigProvider<FileLogSettings>(ConfigSectionName, new FileLogSettingsValidator());
                StartNewLoggingThread();
            });
        }

        public static void Configure(Func<FileLogSettings> settingsSource)
        {
            configProvider?.Dispose();
            configProvider = new LogConfigProvider<FileLogSettings>(settingsSource, new FileLogSettingsValidator());
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
            var eventsCount = eventsBuffer.Drain(currentEvents, 0, currentEvents.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentEvents[i];
                writer.Write(settings.ConversionPattern.Format(currentEvent));
            }
            writer.Flush();
        }

        private static ILogConfigProvider<FileLogSettings> configProvider;

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private const int Capacity = 10000;
        private const string ConfigSectionName = "fileLogConfig";
    }
}