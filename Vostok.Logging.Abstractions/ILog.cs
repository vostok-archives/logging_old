namespace Vostok.Logging.Abstractions
{
    public interface ILog
    {
        void Log(LogEvent @event);

        bool IsEnabledFor(LogLevel level);

        ILog ForContext(string context);
    }
}