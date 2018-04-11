using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Vostok.Logging.Configuration
{
    internal class XmlConfigSection //TODO(mylov) Check unusual xml document structures
    {
        public IReadOnlyDictionary<string, string> Settings { get; }

        public XmlConfigSection(string sectionName)
        {
            Settings = new Dictionary<string, string>();
            try
            {
                if (sectionName == null)
                    return;

                var settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                var configPath = $"{AppDomain.CurrentDomain.FriendlyName}.config";
                using (var file = File.Open(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = XmlReader.Create(file))
                {
                    if (!TryFindSection(reader, sectionName))
                        return;

                    while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(sectionName)))
                    {
                        reader.Read();
                        if (reader.NodeType != XmlNodeType.Element)
                            continue;

                        var key = reader.Name;
                        var value = reader.GetAttribute("value");

                        if (value == null)
                            continue;

                        if (settings.ContainsKey(key))
                            settings[key] = value;
                        else
                            settings.Add(key, value);

                        Settings = settings;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static bool TryFindSection(XmlReader reader, string sectionName)
        {
            while (true)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(sectionName))
                {
                    if (reader.IsEmptyElement)
                        return false;

                    return true;
                }

                reader.Read();

                if (reader.ReadState == ReadState.EndOfFile)
                    return false;
            }
        }

        public override string ToString()
        {
            return string.Join(", ", Settings.Select(p => $"{p.Key}={p.Value}"));
        }
    }
}