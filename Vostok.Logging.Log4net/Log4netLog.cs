using System;
using log4net.Core;
using Vostok.Logging.Abstractions;
using ILog = Vostok.Logging.Abstractions.ILog;

namespace Vostok.Logging.Log4net
{
    public class Log4netLog : ILog
    {
        private readonly ILogger logger;
        private readonly string context;
        private readonly bool useContextHierarchy;

        public Log4netLog(log4net.ILog log, string context = null, bool useContextHierarchy = true)
            : this(log.Logger, context, useContextHierarchy)
        {
        }

        private Log4netLog(ILogger logger, string context, bool useContextHierarchy)
        {
            this.logger = logger;
            this.context = context ?? "";
            this.useContextHierarchy = useContextHierarchy;
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
            var childContext = GetChildContext(context);
            if (childContext == this.context)
                return this;
            return new Log4netLog(logger.Repository.GetLogger(childContext), childContext, useContextHierarchy);
        }

        private string GetChildContext(string context)
        {
            if (!useContextHierarchy || string.IsNullOrEmpty(this.context))
                return context ?? "";
            if (string.IsNullOrEmpty(context))
                return this.context;
            return $"{this.context}.{context}";
        }
    }
}