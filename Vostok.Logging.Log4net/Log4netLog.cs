using System;
using log4net.Core;
using Vostok.Logging.Abstractions;
using ILog = Vostok.Logging.Abstractions.ILog;

namespace Vostok.Logging.Log4net
{
    public class Log4netLog : ILog
    {
        private readonly ILogger logger;

        public Log4netLog(log4net.ILog log)
            : this(log.Logger)
        {
        }

        private Log4netLog(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(LogEvent @event)
        {
            if (!IsEnabledFor(@event.Level))
                return;
            logger.Log(TranslateEvent(@event));
        }

        private LoggingEvent TranslateEvent(LogEvent @event)
        {
            var loggingEvent = new LoggingEvent(
                null,
                null,
                logger.Name,
                TranslateLevel(@event.Level),
                LogEventFormatter.FormatMessage(@event.MessageTemplate, @event.Properties), @event.Exception);
            // todo create loggingEventData manually - just to set timestamp correctly
            foreach (var property in @event.Properties)
                loggingEvent.Properties[property.Key] = property.Value;
            return loggingEvent;
        }

        private Level TranslateLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return logger.IsEnabledFor(TranslateLevel(level));
        }

        public ILog ForContext(string context)
        {
            return new Log4netLog(logger.Repository.GetLogger(context));
        }
    }
}