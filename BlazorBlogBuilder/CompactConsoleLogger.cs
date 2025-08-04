using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace SilentOrbit.BlazorBlogBuilder;

class CompactConsoleLogger<T> : CompactConsoleLogger
{
    public CompactConsoleLogger() : base(typeof(T).Name)
    {
    }
}

class CompactConsoleLogger : ILogger
{
    readonly string category;
    readonly List<Scope> scope = new();

    public CompactConsoleLogger(string category)
    {
        this.category = category;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var s = new Scope(this, state.ToString()!);
        scope.Add(s);
        return s;
    }

    class Scope(CompactConsoleLogger logger, string name) : IDisposable
    {
        public override string ToString() => name;

        void IDisposable.Dispose()
        {
            Debug.Assert(logger.scope[^1] == this);
            logger.scope.Remove(this);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.ResetColor();
        if (Console.CursorLeft != 0)
            Console.WriteLine();

        Console.ForegroundColor = logLevel switch
        {
            LogLevel.None => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Trace => ConsoleColor.Cyan,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Magenta,
            _ => ConsoleColor.Magenta,
        };
        Console.Write(logLevel switch
        {
            LogLevel.None => "none.",
            LogLevel.Debug => "debug",
            LogLevel.Trace => "trace",
            LogLevel.Information => "info.",
            LogLevel.Warning => "warn.",
            LogLevel.Error => "error",
            LogLevel.Critical => "crit.",
            _ => logLevel.ToString(),
        });
        Console.ResetColor();
        Console.Write(" ");

        var debug = logLevel == LogLevel.Debug;
        if (debug)
            Console.ForegroundColor = ConsoleColor.DarkGray;

        if (!debug) Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(category);
        foreach (var s in scope)
        {
            if (!debug) Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(".");
            if (!debug) Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(s);
        }
        if (!debug) Console.ResetColor();
        Console.Write(" ");

        var message = formatter(state, exception);
        if (Console.CursorLeft + message.Length > Console.WindowWidth)
            Console.WriteLine();
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

[UnsupportedOSPlatform("browser")]
[ProviderAlias("ColorConsole")]
sealed class CompactConsoleLoggerProvider : ILoggerProvider
{
    readonly ConcurrentDictionary<string, ILogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new CompactConsoleLogger(name));

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public static class CompactConsoleLoggerExtensions
{
    public static IServiceCollection AddCompactConsoleLogger(this IServiceCollection services)
    {
        services.AddLogging(Configure);
        return services;
    }

    static void Configure(ILoggingBuilder builder)
    {
        builder.AddCompactConsoleLogger();
    }

    public static ILoggingBuilder AddCompactConsoleLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, CompactConsoleLoggerProvider>());

        return builder;
    }

}
