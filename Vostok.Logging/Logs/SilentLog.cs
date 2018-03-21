namespace Vostok.Logging.Logs
{
    public class SilentLog : ILog
    {
        public void Log(LogEvent @event) { }

        public bool IsEnabledFor(LogLevel level) => false;
    }
}