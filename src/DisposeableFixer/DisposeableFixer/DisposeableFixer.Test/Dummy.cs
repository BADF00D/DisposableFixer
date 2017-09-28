using System;
using Microsoft.Extensions.Logging;

namespace UsingAspDotNetILoggerFactory
{
    public class SomeClass
    {
        public SomeClass()
        {
            var factory = new LoggerFactory();
            var provider = new LoggerProvider();
            factory.AddConsole();

            factory.AddProvider(provider);

            provider.Dispose();

        }

        private class LoggerFactory : ILoggerFactory
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public ILogger CreateLogger(string categoryName)
            {
                throw new NotImplementedException();
            }

            public void AddProvider(ILoggerProvider provider)
            {
                throw new NotImplementedException();
            }
        }

        private class LoggerProvider : ILoggerProvider
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public ILogger CreateLogger(string categoryName)
            {
                throw new NotImplementedException();
            }
        }
    }
}

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
}