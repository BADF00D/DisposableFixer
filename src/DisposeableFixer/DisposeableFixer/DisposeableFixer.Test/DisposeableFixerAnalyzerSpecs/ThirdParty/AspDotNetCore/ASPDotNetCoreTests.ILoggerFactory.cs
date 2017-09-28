using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.AspDotNetCore
{
    internal partial class ASPDotNetCoreTests
    {
        private const string LoggerFactoryInterfaces = @"
namespace Microsoft.Extensions.Logging {
    public interface IConsoleLoggerSettings {
        bool IncludeScopes { get; }
        //IChangeToken ChangeToken { get; }
        bool TryGetSwitch(string name, out LogLevel level);
        IConsoleLoggerSettings Reload();
    }
    public interface ILoggingBuilder {
        //IServiceCollection Services { get; }
    }
    public class ConsoleLoggerOptions {
        public bool IncludeScopes { get; set; } = false;
    }
    public static class ConsoleLoggerExtensions {
        public static ILoggingBuilder AddConsole(this ILoggingBuilder builder) {
            throw new NotImplementedException();
        }

        public static ILoggingBuilder AddConsole(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> configure) {
            throw new NotImplementedException();
        }

        public static ILoggerFactory AddConsole(this ILoggerFactory factory) {
            throw new NotImplementedException();
        }
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, bool includeScopes) {
            throw new NotImplementedException();
        }

        public static ILoggerFactory AddConsole(this ILoggerFactory factory, LogLevel minLevel) {
            throw new NotImplementedException();
        }

        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            LogLevel minLevel,
            bool includeScopes) {
            throw new NotImplementedException();
        }
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter) {
            throw new NotImplementedException();
        }
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter,
            bool includeScopes) {
            throw new NotImplementedException();
        }
        public static ILoggerFactory AddConsole(
            this ILoggerFactory factory,
            IConsoleLoggerSettings settings) {
            throw new NotImplementedException();
        }

        public static ILoggerFactory AddConsole(this ILoggerFactory factory, IConfiguration configuration) {
            throw new NotImplementedException();
        }
    }
    public interface IConfiguration {
        string this[string key] { get; set; }
        //IConfigurationSection GetSection(string key);

        //IEnumerable<IConfigurationSection> GetChildren();

        //IChangeToken GetReloadToken();
    }
    public interface ILogger {
        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter);

        bool IsEnabled(LogLevel logLevel);
        IDisposable BeginScope<TState>(TState state);
    }

    public enum LogLevel {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

    public struct EventId {
        public EventId(int id, string name = null) {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        public static implicit operator EventId(int i) {
            return new EventId(i);
        }

        public override string ToString() {
            if (Name != null) {
                return Name;
            }
            return Id.ToString();
        }
    }

    public interface ILoggerProvider : IDisposable {
        ILogger CreateLogger(string categoryName);
    }

    public interface ILoggerFactory : IDisposable {
        ILogger CreateLogger(string categoryName);
        void AddProvider(ILoggerProvider provider);
    }
}";

        private static TestCaseData ILoggerFactory_AddConsole()
        {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole();

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code+LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole()");
        }

        private static TestCaseData ILoggerFactory_AddConsole_Boolean() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole(true);

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(bool)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_LogLevel() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole(LogLevel.Critical);

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(LogLevel)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_LogLevel_Boolean() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole(LogLevel.Critical, true);

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(LogLevel, boolean)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_Func_string_LogLevel_to_Boolean() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole((val, level) => true);

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(Func<string,LogLevel, boolean>)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_Func_string_LogLevel_to_Boolean_Boolean() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole((val, level) => true, true);

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(Func<string,LogLevel, boolean>, boolean)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_IConsoleLoggerSettings() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole(new Settings());

            factory.AddProvider(provider);

            provider.Dispose();

        }
        private class Settings : IConsoleLoggerSettings
        {
            public bool IncludeScopes { get; }
            public bool TryGetSwitch(string name, out LogLevel level){throw new NotImplementedException();}
            public IConsoleLoggerSettings Reload(){throw new NotImplementedException();}
        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(IConsoleLoggerSettings)");
        }

        private static TestCaseData ILoggerFactory_AddConsole_IConfiguration() {
            const string code = @"
using System;
using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            factory.AddConsole(new Configuration());

            factory.AddProvider(provider);

            provider.Dispose();

        }
        private class Configuration : IConfiguration
        {
            public string this[string key]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose(){throw new NotImplementedException();}
            public ILogger CreateLogger(string categoryName){throw new NotImplementedException();}
            public void AddProvider(ILoggerProvider provider){throw new NotImplementedException();}
        }
    }
}";
            return new TestCaseData(code + LoggerFactoryInterfaces, 0)
                .SetName("ILoggerFactory.AddConsole(IConfiguration)");
        }
    }
}