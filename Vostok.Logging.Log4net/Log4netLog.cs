using log4net.Core;
using Vostok.Logging.Abstractions;
using ILog = Vostok.Logging.Abstractions.ILog;

namespace Vostok.Logging.Log4net
{
    public class Log4netLog : ILog
    {
        private readonly ILogger logger;
        private readonly bool useContextHierarchy;

        public Log4netLog(log4net.ILog log, bool useContextHierarchy = false)
            : this(log.Logger, useContextHierarchy)
        {
        }

        private Log4netLog(ILogger logger, bool useContextHierarchy)
        {
            this.logger = logger;
            this.useContextHierarchy = useContextHierarchy;
        }

        public void Log(LogEvent @event)
        {
            if (!IsEnabledFor(@event.Level))
                return;
            logger.Log(Log4netHelpers.TranslateEvent(logger, @event));
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return logger.IsEnabledFor(Log4netHelpers.TranslateLevel(level));
        }

        public ILog ForContext(string context)
        {
            var loggerName = GetLoggerNameForContext(context);
            if (loggerName == logger.Name)
                return this;
            return new Log4netLog(logger.Repository.GetLogger(loggerName), useContextHierarchy);
        }

        private string GetLoggerNameForContext(string context)
        {
            if (!useContextHierarchy)
                return context ?? "";
            if (string.IsNullOrEmpty(context))
                return logger.Name;
            if (logger.Name == "")
                return context;
            return $"{logger.Name}.{context}";
        }
    }
}