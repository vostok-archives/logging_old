﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.Abstractions;

#pragma warning disable 612

namespace Vostok.Logging.Tests
{
    [TestFixture(LogLevel.Debug)]
    [TestFixture(LogLevel.Info)]
    [TestFixture(LogLevel.Warn)]
    [TestFixture(LogLevel.Error)]
    [TestFixture(LogLevel.Fatal)]
    internal class LogExtensions_Tests
    {
        public LogExtensions_Tests(LogLevel level)
        {
            this.level = level;
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_message()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(string));

            method.Invoke(null, new object[] {log, message});

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties == null &&
                e.Exception == null));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_exception()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(Exception));

            method.Invoke(null, new object[] { log, exception });

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate == null &&
                e.Properties == null &&
                e.Exception == exception));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_exception_message()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(Exception), typeof(string));

            method.Invoke(null, new object[] {log, exception, message});

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties == null &&
                e.Exception == exception));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_message_properties()
        {
            var method = GetLogMethodWithSingleGenericParameter(level, typeof(ILog), typeof(string));
            method = method.MakeGenericMethod(new {A = 1}.GetType());

            method.Invoke(null, new object[] {log, message, new {A = 1}});

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties.SequenceEqual(new Dictionary<string, object>{ { "A", 1 } }) &&
                e.Exception == null));
        }

        [Test]
        public void LogMethod_should_use_non_anonymous_class_argument_like_parameter_for_arguments_message_properies()
        {
            var method = GetLogMethodWithSingleGenericParameter(level, typeof(ILog), typeof(string));
            method = method.MakeGenericMethod(new CustomClass().GetType());

            var obj = new CustomClass();
            method.Invoke(null, new object[] { log, message, obj });

            log.Received(1).Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(new Dictionary<string, object> { { "0", obj } })));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_message_parameters()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(string), typeof(object[]));

            var obj = new object();
            method.Invoke(null, new object[] { log, message, new []{ obj } });

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties.SequenceEqual(new Dictionary<string, object>{ { "0", obj } }) &&
                e.Exception == null));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_exception_message_properties()
        {
            var method = GetLogMethodWithSingleGenericParameter(level, typeof(ILog), typeof(Exception), typeof(string));
            method = method.MakeGenericMethod(new { A = 1 }.GetType());

            method.Invoke(null, new object[] { log, exception, message, new { A = 1 } });

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties.SequenceEqual(new Dictionary<string, object> { { "A", 1 } }) &&
                e.Exception == exception));
        }

        [Test]
        public void LogMethod_should_use_non_anonymous_class_argument_like_parameter_for_arguments_exception_message_properies()
        {
            var method = GetLogMethodWithSingleGenericParameter(level, typeof(ILog), typeof(Exception), typeof(string));
            method = method.MakeGenericMethod(new CustomClass().GetType());

            var obj = new CustomClass();
            method.Invoke(null, new object[] { log, exception, message, obj });

            log.Received(1).Log(Arg.Is<LogEvent>(e => e.Properties.SequenceEqual(new Dictionary<string, object> { { "0", obj } })));
        }

        [Test]
        public void LogMethod_should_work_correctly_for_arguments_exception_message_parameters()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(Exception), typeof(string), typeof(object[]));

            var obj = new object();
            method.Invoke(null, new object[] { log, exception, message, new[] { obj } });

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
                e.MessageTemplate.Equals(message) &&
                e.Properties.SequenceEqual(new Dictionary<string, object> { { "0", obj } }) &&
                e.Exception == exception));
        }

        [Test]
        public void ObsoleteLogMethod_should_work_correctly_for_arguments_message_exception()
        {
            var method = GetLogMethod(level, typeof(ILog), typeof(string), typeof(Exception));

            method.Invoke(null, new object[] {log, message, exception});

            log.Received(1).Log(Arg.Is<LogEvent>(e =>
                e.Level == level &&
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

        private static MethodInfo GetLogMethod(LogLevel level, params Type[] argumentsTypes)
        {
            var levelMethods = typeof(LogExtensions).GetMethods().Where(m => m.Name.Equals(level.ToString()));
            return levelMethods.Single(
                m =>
                {
                    if (m.ContainsGenericParameters)
                        return false;

                    var parameters = m.GetParameters().Select(p => p.ParameterType);
                    return parameters.SequenceEqual(argumentsTypes);
                });
        }

        private static MethodInfo GetLogMethodWithSingleGenericParameter(LogLevel level, params Type[] argumentsTypes)
        {
            var levelMethods = typeof(LogExtensions).GetMethods().Where(m => m.Name.Equals(level.ToString()));
            return levelMethods.Single(
                m =>
                {
                    var parameters = m.GetParameters().Select(p => p.ParameterType).ToList();

                    if (parameters.Count(p => p.IsGenericParameter) != 1)
                        return false;

                    if (!parameters.Last().IsGenericParameter)
                        return false;

                    return parameters.Where(p => !p.IsGenericParameter).SequenceEqual(argumentsTypes);
                });
        }

        private ILog log;
        private Exception exception;
        private string message;

        private readonly LogLevel level;

        private class CustomClass { }
    }
}