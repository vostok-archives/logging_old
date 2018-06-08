using System.Linq;

namespace Vostok.Logging.Abstractions.Logs
{
    public class CompositeLog : ILog
    {
        private readonly ILog[] baseLogs;
        
        public CompositeLog(params ILog[] baseLogs)
        {
            this.baseLogs = baseLogs;
        }

        public void Log(LogEvent @event)
        {
            foreach (var baseLog in baseLogs)
                baseLog.Log(@event);
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return baseLogs.Any(x => x.IsEnabledFor(level));
        }

        public ILog ForContext(string context)
        {
            return new CompositeLog(baseLogs.Select(x => x.ForContext(context)).ToArray());
        }
    }

}