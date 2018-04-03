using System;
using System.IO;
using System.Text;
using System.Threading;
using Vostok.Commons.ThreadManagment;
using Vostok.Logging.Configuration;

namespace Vostok.Logging.Logs
{
    public class FileLog : ILog
    {
        public FileLog()
        {
            var configProvider = new FileLogConfigProvider();
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
                var filePath = "D:/log";

                var encoding = Encoding.UTF8;
                var datePattern = "HH:mm:ss.fff";

                var date = DateTimeOffset.UtcNow.Date;

                var file = File.Open($"{filePath}{date.ToString(datePattern)}", FileMode.Append, FileAccess.Write, FileShare.Read);
                var writer = new StreamWriter(file, encoding);

                while (true)
                {
                    var currentDate = DateTimeOffset.UtcNow.Date;
                    if (currentDate > date)
                    {
                        writer.Close();
                        file = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                        writer = new StreamWriter(file, encoding);
                    }
                    try
                    {
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
                        file = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                        writer = new StreamWriter(file, encoding);
                    }
                }
            });
        }

        private static void WriteEventsToFile(TextWriter writer)
        {
            var eventsCount = eventsBuffer.Drain(currentEvents, 0, currentEvents.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentEvents[i];
                writer.Write(FormatEvent(currentEvent));
            }
            writer.Flush();
        }

        private static string FormatEvent(LogEvent @event)
        {
            var message = LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties);
            return $"{@event.Timestamp:HH:mm:ss.fff zzz} {@event.Level} {message} {@event.Exception}{Environment.NewLine}";
        }

        private static readonly LogEvent[] currentEvents = new LogEvent[Capacity];
        private static readonly BoundedBuffer<LogEvent> eventsBuffer = new BoundedBuffer<LogEvent>(Capacity);

        private const int Capacity = 10000;
    }
}