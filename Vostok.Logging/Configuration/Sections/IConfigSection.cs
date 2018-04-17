using System.Collections.Generic;

namespace Vostok.Logging.Configuration.Sections
{
    public interface IConfigSection //TODO(mylov): Should be internal :(
    {
        IReadOnlyDictionary<string, string> Settings { get; }
    }
}