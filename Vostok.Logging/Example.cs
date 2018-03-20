using System;
using System.Net;

namespace Vostok.Logging
{
    public class Example
    {
        public void Main()
        {
            var log = new FakeLog();

            log.Info(
                "Hello from {application}!",
                new {Application = AppDomain.CurrentDomain.FriendlyName});
        }

        // From Kanso:
        private void LogSuccessfulShrink(ILog log, long lengthBefore, long lengthAfter, TimeSpan elapsed, IPAddress initiator, Guid shrinkId)
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