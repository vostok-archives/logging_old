using System;
using System.IO;
using System.Threading;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration;

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
                var writer = CreateFileWriter();
                while (true)
                {
                    try
                    {
                        if (configProvider.Settings.EnableRolling)
                        {
                            var currentDate = DateTimeOffset.UtcNow.Date;
                            if (currentDate > startDate)
                            {
                                writer.Close();
                                writer = CreateFileWriter();
                            }
                        }

                        WriteEventsToFile(writer);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(300);
                    }

                    if (eventsBuffer.Count == 0)
                    {
                        writer.Close();
                        eventsBuffer.WaitForNewItems();
                        writer = CreateFileWriter();
                    }
                }
            });
        }

        private static TextWriter CreateFileWriter()
        {
            var settings = configProvider.Settings;

            var fileName = settings.EnableRolling 
                ? $"{settings.FilePath}{DateTimeOffset.UtcNow.Date:yyyy.MM.dd}" 
                : settings.FilePath;

            var fileMode = settings.AppendToFile 
                ? FileMode.Append 
                : FileMode.OpenOrCreate;

            var file = File.Open(fileName, fileMode, FileAccess.Write, FileShare.Read);
            return new StreamWriter(file, settings.Encoding);
        }

        private static void WriteEventsToFile(TextWriter writer)
        {
            var eventsCount = eventsBuffer.Drain(currentEvents, 0, currentEvents.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentEvents[i];
                writer.Write(configProvider.Settings.ConversionPattern.Format(currentEvent));
            }
            writer.Flush();
        }

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private static readonly FileLogConfigProvider configProvider;

        private const int Capacity = 10000;
    }
}