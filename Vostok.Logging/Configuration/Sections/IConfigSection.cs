using System.Collections.Generic;

namespace Vostok.Logging.Configuration.Sections
{
    internal interface IConfigSection
    {
        IReadOnlyDictionary<string, string> Settings { get; }
    }
}