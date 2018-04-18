using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Configuration;
using Vostok.Logging.Configuration.Settings;

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class SettingsEquals_Tests
    {
        [Test]
        public void Settings_equals_method_should_use_all_settings_properties_for_check()
        {
            CheckBySteps<FileLogSettings>();
            CheckBySteps<ConsoleLogSettings>();
        }

        public void CheckBySteps<TSettings>() where TSettings : new()
        {
            var settings1 = GenerateSettings<TSettings>();
            var settings2 = GenerateSettings<TSettings>();

            settings1.Should().Be(settings2);

            foreach (var property in typeof (TSettings).GetProperties())
            {
                UpdatePropertyState(property, settings1);
                settings1.Should().NotBe(settings2);
                UpdatePropertyState(property, settings2);
                settings1.Should().Be(settings2);
            }
        }

        private void UpdatePropertyState(PropertyInfo property, object obj)
        {
            if(!typeToStates.TryGetValue(property.PropertyType, out var states))
                throw new NotSupportedPropertyTypeException(property);

            property.SetValue(obj, states.updatedState);
        }

        private TSettings GenerateSettings<TSettings>() where TSettings : new()
        {
            var settings = new TSettings();

            foreach (var property in settings.GetType().GetProperties())
            {
                if(!typeToStates.TryGetValue(property.PropertyType, out var states))
                    throw new NotSupportedPropertyTypeException(property);

                property.SetValue(settings, states.baseState);
            }

            return settings;
        }

        private readonly Dictionary<Type, (object baseState, object updatedState)> typeToStates = 
            new Dictionary<Type, (object, object)>
            {
                {typeof(string), ($"{Guid.NewGuid().ToString().Substring(0, 8)}", $"{Guid.NewGuid().ToString().Substring(0, 8)}") },
                {typeof(ConversionPattern), (ConversionPattern.FromString("%d %l %m"), ConversionPattern.FromString("%m %l %d")) },
                {typeof(bool), (true, false) },
                {typeof(Encoding), (Encoding.UTF8, Encoding.Unicode) }
            };

        private class NotSupportedPropertyTypeException : Exception
        {
            public NotSupportedPropertyTypeException(PropertyInfo property) : base($"property: {property.Name}, type: {property.PropertyType}") { }
        }
    }
}