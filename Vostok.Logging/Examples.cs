using System;
using System.Net;
using Vostok.Logging.Logs;

namespace Vostok.Logging
{
    public class Examples
    {
        public void Example1()
        {
            var log = new SilentLog();
            log.Info("Hello from {application}!", new {Application = AppDomain.CurrentDomain.FriendlyName});
        }

        // From Kanso:
        private void Example2_LogSuccessfulShrink(ILog log, long lengthBefore, long lengthAfter, TimeSpan elapsed, IPAddress initiator, Guid shrinkId)
        {
            // TODO(krait): File log should be configurable with layout
            log.Info(
                "Completed operation log shrink in {elapsed} by request from {initiator}. Size before = {lengthBefore}. Size after = {lengthAfter}. Shrink id = {shrinkId}.",
                new
                {
                    elapsed,
                    initiator,
                    lengthBefore,
                    lengthAfter,
                    shrinkId
                });
        }
    }
}