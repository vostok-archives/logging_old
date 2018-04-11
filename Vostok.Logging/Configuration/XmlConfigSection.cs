using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Vostok.Logging.Configuration
{
    internal class XmlConfigSection
    {
        public IReadOnlyDictionary<string, string> Settings { get; private set; }

        public XmlConfigSection(string sectionName)
        {
            this.sectionName = sectionName;
            Update();
        }

        public void Update()
        {
            Settings = new Dictionary<string, string>();

            try
            {
                if (sectionName == null)
                    return;

                var dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                var configPath = $"{AppDomain.CurrentDomain.FriendlyName}.config";
                var fileReader = File.Open(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var reader = XmlReader.Create(fileReader);

                var result = TryFindSection(reader);

                if (!result)
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

                    if (dict.ContainsKey(key))
                        dict[key] = value;
                    else
                        dict.Add(key, value);

                    Settings = dict;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool TryFindSection(XmlReader reader)
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

        private readonly string sectionName;
    }
}