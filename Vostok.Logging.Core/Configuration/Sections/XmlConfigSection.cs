using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Vostok.Logging.Core.Configuration.Sections
{
    // CR(krait): Please delete all unused code.
    internal class XmlConfigSection : IConfigSection
    {
        public IReadOnlyDictionary<string, string> Settings { get; }

        public XmlConfigSection(string sectionName, string configPath)
        {
            Settings = new Dictionary<string, string>();
            try
            {
                if (sectionName == null)
                    return;

                var settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                using (var file = File.Open(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = XmlReader.Create(file))
                {
                    if (!TryFindSection(reader, sectionName))
                        return;

                    var settingsDepth = -1;

                    while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(sectionName)))
                    {
                        reader.Read();
                        if (reader.NodeType != XmlNodeType.Element)
                            continue;

                        if (settingsDepth < 0)
                            settingsDepth = reader.Depth;

                        if(settingsDepth != reader.Depth)
                            continue;

                        var key = reader.Name;
                        var value = reader.GetAttribute("value");

                        if (value == null)
                            continue;

                        if (settings.ContainsKey(key))
                            settings[key] = value;
                        else
                            settings.Add(key, value);
                    }

                    Settings = settings;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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