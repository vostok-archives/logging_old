using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Extensions
{
    public static class LogExtensions
    {
        public static void Info(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, message));
        }

        [Obsolete]
        public static void Info(this ILog log, string message, Exception exception)
        {
            log.Info(exception, message);
        }

        public static void Info(this ILog log, Exception exception)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, null, exception: exception));
        }

        public static void Info(this ILog log, Exception exception, string message)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, message, exception: exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info<T>(this ILog log, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Info(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info<T>(this ILog log, Exception exception, string messageTemplate, T properties) where T : class
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Info(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }
    }
}