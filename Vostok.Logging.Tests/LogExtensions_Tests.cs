using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.Extensions;
#pragma warning disable 612

namespace Vostok.Logging.Tests
{
    [TestFixture]
    internal class LogExtensions_Tests
    {
        [Test]
        public void Info_with_anonymous_class_argument_should_use_it_like_properties()
        {
            log.Info(message, new { A = 1 });

            log.Received().Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(new Dictionary<string, object>{{ "A", 1 }})));
        }

        [Test]
        public void Info_with_single_object_argument_should_use_it_like_parameter()
        {
            var parametersDict = new Dictionary<string, object>
            {
                {"0", new object() }
            };

            log.Info(message, parametersDict["0"]);

            log.Received().Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(parametersDict)));
        }

        [Test]
        public void Info_with_several_object_arguments_should_use_them_like_parameters()
        {
            var parametersDict = new Dictionary<string, object>
            {
                {"0", new object() },
                {"1", new object() }
            };

            log.Info(message, parametersDict["0"], parametersDict["1"]);

            log.Received().Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(parametersDict)));
        }

        [Test]
        public void Info_with_single_CustomCluss_argument_should_use_it_like_parameter()
        {
            var parametersDict = new Dictionary<string, object>
            {
                {"0", new CustomClass() }
            };

            log.Info(message, parametersDict["0"]);

            log.Received().Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(parametersDict)));
        }

        [Test]
        public void Info_with_several_CustomCluss_arguments_should_use_them_like_parameters()
        {
            var parametersDict = new Dictionary<string, object>
            {
                {"0", new CustomClass() },
                {"1", new CustomClass() }
            };

            log.Info(message, parametersDict["0"], parametersDict["1"]);

            log.Received().Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(parametersDict)));
        }

        [Test]
        public void ObsoleteInfo_should_create_correct_log_event_for_log_method()
        {
            log.Info(message, exception);
            log.Received().Log(Arg.Is<LogEvent>(e =>
                e.Level == LogLevel.Info &&
                e.MessageTemplate.Equals(message) &&
                e.Properties == null &&
                e.Exception == exception));
        }

        [SetUp]
        public void SetUp()
        {
            log = Substitute.For<ILog>();
            exception = new Exception();
            message = "message";
        }

        private ILog log;
        private Exception exception;
        private string message;

        private class CustomClass { }
    }
}