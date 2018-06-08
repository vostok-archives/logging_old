namespace Vostok.Logging.Abstractions
{
    public interface ILog
    {
        /// <summary>
        /// Logs the given <see cref="LogEvent"/>. This method should not be used in most cases. Use one of the <c>Debug</c>, <c>Info</c>, <c>Warn</c>, <c>Error</c> or <c>Fatal</c> extension methods.
        /// </summary>
        void Log(LogEvent @event);

        /// <summary>
        /// <para>Returns whether the current logger is configured to log events of the given <see cref="LogLevel"/>.</para>
        /// <para>In case you use the <see cref="Log"/> method directly, call this method to avoid unnecessary construction of <see cref="LogEvent"/>s.</para>
        /// </summary>
        bool IsEnabledFor(LogLevel level);
    }
}