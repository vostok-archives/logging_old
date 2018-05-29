using System.Collections.Generic;

namespace Vostok.Logging.Core.Configuration.Sections
{
    internal interface IConfigSection
    {
        IReadOnlyDictionary<string, string> Settings { get; }
    }
}