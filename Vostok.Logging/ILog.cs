using System.Net;

namespace Vostok.Logging
{
    public interface ILog
    {
        void Log(LogEvent @event);

        bool IsEnabledFor(LogLevel level);
    }

    class HostLog : ILog
    {
        private ILog logImplementation;

        public void Log(LogEvent @event)
        {
            logImplementation.Log(@event.SetProperty("hostname", Dns.GetHostName()));
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return logImplementation.IsEnabledFor(level);
        }
    }
}