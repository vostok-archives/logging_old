using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Extensions
{
    public static class LogExtensions
    {
        #region Debug
        public static void Debug(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, message));
        }

        public static void Debug(this ILog log, Exception exception)
        {
            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, null, exception: exception));
        }

        public static void Debug(this ILog log, Exception exception, string message)
        {
            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, message, exception: exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Debug<T>(this ILog log, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Debug(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Debug(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Debug<T>(this ILog log, Exception exception, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Debug(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Debug(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Debug, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }

        [Obsolete]
        public static void Debug(this ILog log, string message, Exception exception)
        {
            log.Debug(exception, message);
        }
        #endregion
        #region Info
        public static void Info(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, message));
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
        public static void Info(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info<T>(this ILog log, Exception exception, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Info(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Info(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Info, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }

        [Obsolete]
        public static void Info(this ILog log, string message, Exception exception)
        {
            log.Info(exception, message);
        }
        #endregion
        #region Warn
        public static void Warn(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, message));
        }

        public static void Warn(this ILog log, Exception exception)
        {
            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, null, exception: exception));
        }

        public static void Warn(this ILog log, Exception exception, string message)
        {
            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, message, exception: exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Warn<T>(this ILog log, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Warn(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Warn(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Warn<T>(this ILog log, Exception exception, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Warn(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Warn(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Warn, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }

        [Obsolete]
        public static void Warn(this ILog log, string message, Exception exception)
        {
            log.Warn(exception, message);
        }
        #endregion
        #region Error
        public static void Error(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, message));
        }

        public static void Error(this ILog log, Exception exception)
        {
            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, null, exception: exception));
        }

        public static void Error(this ILog log, Exception exception, string message)
        {
            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, message, exception: exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Error<T>(this ILog log, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Error(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Error(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Error<T>(this ILog log, Exception exception, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Error(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Error(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Error, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }

        [Obsolete]
        public static void Error(this ILog log, string message, Exception exception)
        {
            log.Error(exception, message);
        }
        #endregion
        #region Fatal
        public static void Fatal(this ILog log, string message)
        {
            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, message));
        }

        public static void Fatal(this ILog log, Exception exception)
        {
            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, null, exception: exception));
        }

        public static void Fatal(this ILog log, Exception exception, string message)
        {
            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, message, exception: exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Fatal<T>(this ILog log, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Fatal(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Fatal(this ILog log, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary()));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Fatal<T>(this ILog log, Exception exception, string messageTemplate, T properties)
        {
            if (!properties.GetType().IsConstructedGenericType)
            {
                log.Fatal(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, messageTemplate, properties.ToDictionary(), exception));
        }

        [StringFormatMethod("messageTemplate")]
        public static void Fatal(this ILog log, Exception exception, string messageTemplate, params object[] parameters)
        {
            log.Log(new LogEvent(LogLevel.Fatal, DateTimeOffset.UtcNow, messageTemplate, parameters.ToDictionary(), exception));
        }

        [Obsolete]
        public static void Fatal(this ILog log, string message, Exception exception)
        {
            log.Fatal(exception, message);
        }
        #endregion
    }
}