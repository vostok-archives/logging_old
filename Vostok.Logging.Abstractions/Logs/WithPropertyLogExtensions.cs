namespace Vostok.Logging.Abstractions.Logs
{
    public static class WithPropertyLogExtensions
    {
        public static ILog WithProperty<T>(this ILog log, string key, T value)
        {
            return new WithPropertyLog<T>(log, key, value);
        }

        private class WithPropertyLog<T> : ILog
        {
            private readonly ILog baseLog;
            private readonly string key;
            private readonly T value;

            public WithPropertyLog(ILog baseLog, string key, T value)
            {
                this.baseLog = baseLog;
                this.key = key;
                this.value = value;
            }

            public void Log(LogEvent @event)
            {
                baseLog.Log(@event.WithProperty(key, value));
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return baseLog.IsEnabledFor(level);
            }

            public ILog ForContext(string context)
            {
                return new WithPropertyLog<T>(baseLog.ForContext(context), key, value);
            }
        }
    }
}