using System;
using System.IO;
using System.Threading;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration;
using Vostok.Logging.Configuration.Settings;

namespace Vostok.Logging.Logs
{
    public class FileLog : ILog
    {
        static FileLog()
        {
            configProvider = new LogConfigProvider<FileLogSettings>(ConfigSectionName);
            StartNewLoggingThread();
        }

        public void Configure(Func<FileLogSettings> settingsSource)
        {
            configProvider = new LogConfigProvider<FileLogSettings>(settingsSource);
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

            if (newSettings.FilePath.Equals(settings.FilePath, StringComparison.CurrentCultureIgnoreCase) &&
                newSettings.ConversionPattern.PatternStr.Equals(settings.ConversionPattern.PatternStr, StringComparison.CurrentCultureIgnoreCase) &&
                newSettings.EnableRolling == settings.EnableRolling &&
                newSettings.AppendToFile == settings.AppendToFile &&
                newSettings.Encoding.Equals(settings.Encoding))
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