namespace Vostok.Logging
{
    public interface ILog
    {
        void Log(LogEvent @event);

        // TODO(krait): IsEnabled(LogLevel)
    }
}