namespace Vostok.Logging
{
    public class SilentLog : ILog
    {
        public void Log(LogEvent @event)
        {
        }

        public bool IsEnabledFor(LogLevel level) => false;
    }
}