using System;
using System.IO;
using System.Threading;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration.FileLog;

namespace Vostok.Logging.Logs
{
    public class FileLog : ILog
    {
        static FileLog()
        {
            configProvider = new FileLogConfigProvider();
            StartNewLoggingThread();
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

        private static bool TryUpdateSettings(ref FileLogSettings settings)
        {
            var newSettings = configProvider.Settings;
            if (!settings.Equals(newSettings))
            {
                settings = newSettings;
                return true;
            }

            return false;
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

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private static readonly IFileLogConfigProvider configProvider;

        private const int Capacity = 10000;
    }
}