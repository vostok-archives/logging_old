using System.Configuration;

namespace Vostok.Logging.Configuration
{
    internal class FileLogConfigElement : ConfigurationElement
    {
        [ConfigurationProperty(TypePropertyName, DefaultValue = "", IsKey = true, IsRequired = false)]
        public string Type => (string)base[TypePropertyName];

        [ConfigurationProperty(ValuePropertyName, DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Value => (string)base[ValuePropertyName];

        private const string TypePropertyName = "type";
        private const string ValuePropertyName = "value";
    }
}