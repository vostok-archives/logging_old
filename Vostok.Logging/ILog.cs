namespace Vostok.Logging
{
    public interface ILog
    {
        void Log(LogEvent @event);

        bool IsEnabledFor(LogLevel level);
    }
}