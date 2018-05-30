using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions.Extensions;
using Vostok.Logging.Core.Configuration.Sections;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class XmlConfigSection_Tests
    {
        [Test]
        public void Section_should_contains_all_correct_settings_from_xmlconfig()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(2);
        }

        [Test]
        public void Section_should_contains_settings_only_from_first_depth_level()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    using (b.Tag("option1", new {value = "value1"}))
                    {
                        b.SingleLineTag("option1.1", new {value = "value1.1"});
                    }
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Keys.Should().BeEquivalentTo("option1", "option2");
        }

        [Test]
        public void Section_should_not_contains_settings_if_xmlconfig_not_contains_section_with_such_name()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(AbsentSectionName, configFileName);

            section.Settings.Count.Should().Be(0);
        }

        [Test]
        public void Section_should_not_contains_settings_if_xmlconfig_not_found()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, $"{Guid.NewGuid().ToString().Substring(0, 8)}.config");

            section.Settings.Count.Should().Be(0);
        }

        [Test]
        public void Section_should_contains_only_settings_with_value_attribute()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { type = "type2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(1);
        }

        [Test]
        public void Section_should_rewrite_doubled_setting()
        {
            Config = CreateXmlConfigContent(b => {
                using (b.Tag(CustomSectionName))
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option1", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(1);
            section.Settings["option1"].Should().Be("value2");
        }

        [Test]
        public void Section_should_not_contains_settings_if_it_is_single_tag()
        {
            Config = CreateXmlConfigContent(b =>
            {
                b.SingleLineTag(CustomSectionName);
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(0);
        }

        [Test]
        public void Section_should_not_contains_settings_if_it_has_only_start_tag()
        {
            Config = CreateXmlConfigContent(b =>
            {
                b.OpenTag(CustomSectionName);
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(0);
        }

        [Test]
        public void Section_should_not_contains_settings_if_it_has_only_end_tag()
        {
            Config = CreateXmlConfigContent(b =>
            {
                {
                    b.SingleLineTag("option1", new { value = "value1" });
                    b.SingleLineTag("option2", new { value = "value2" });
                }
                b.CloseTag(CustomSectionName);
            });

            var section = new XmlConfigSection(CustomSectionName, configFileName);

            section.Settings.Count.Should().Be(0);
        }

        [SetUp]
        public void SetUp()
        {
            configFileName = $"{Guid.NewGuid().ToString().Substring(0, 8)}.config";
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(configFileName))
                File.Delete(configFileName);
        }

        private static string CreateXmlConfigContent(Action<StringBuilder> xmlConfigBuilder)
        {
            var builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            using (builder.Tag("configuration"))
            {
                xmlConfigBuilder(builder);
            }
            return builder.ToString();
        }

        private string configFileName;

        private string Config
        {
            set
            {
                using (var file = File.Open(configFileName, FileMode.CreateNew, FileAccess.Write))
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(value);
                }
            }
        }

        private const string CustomSectionName = "customSection";
        private const string AbsentSectionName = "absentSection";
    }

    internal static class XmlTagsPlaceHelper
    {
        public static TagContext Tag(this StringBuilder builder, string tagName, object attributes = null)
        {
            return new TagContext(builder, tagName, attributes);
        }

        public static void OpenTag(this StringBuilder builder, string tagName, object attributes = null)
        {
            builder.AppendLine(attributes != null
                ? $"<{tagName} {string.Join(" ", attributes.ToDictionary().Select(p => $"{p.Key}=\"{p.Value}\""))}>"
                : $"<{tagName}>");
        }

        public static void CloseTag(this StringBuilder builder, string tagName)
        {
            builder.AppendLine($"</{tagName}>");
        }

        public static void SingleLineTag(this StringBuilder builder, string tagName, object attributes = null)
        {
            builder.AppendLine(attributes != null
                ? $"<{tagName} {string.Join(" ", attributes.ToDictionary().Select(p => $"{p.Key}=\"{p.Value}\""))} />"
                : $"<{tagName} />");
        }

        internal class TagContext : IDisposable
        {
            public TagContext(StringBuilder builder, string tagName, object attributes = null)
            {
                this.builder = builder;
                this.tagName = tagName;
                builder.OpenTag(tagName, attributes);
            }

            public void Dispose()
            {
                builder.CloseTag(tagName);
            }

            private readonly StringBuilder builder;
            private readonly string tagName;
        }
    }
}