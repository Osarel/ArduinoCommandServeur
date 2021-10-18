using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Robot.Logger;

public sealed class ConsoleLoggerProvider : ILoggerProvider
{
    private static Dictionary<LogLevel, LogLevelConfiguration> config = new Dictionary<LogLevel, LogLevelConfiguration>();
    private readonly ConcurrentDictionary<string, ILoggerServer> _loggers = new ConcurrentDictionary<string, ILoggerServer>();

    public ConsoleLoggerProvider()
    {
        config.Add(LogLevel.Debug, new LogLevelConfiguration(ConsoleColor.Black));
        config.Add(LogLevel.Information, new LogLevelConfiguration(ConsoleColor.Green));
        config.Add(LogLevel.Error, new LogLevelConfiguration(ConsoleColor.Red));
        config.Add(LogLevel.Trace, new LogLevelConfiguration(ConsoleColor.DarkCyan));
        config.Add(LogLevel.Warning, new LogLevelConfiguration(ConsoleColor.Yellow));
        config.Add(LogLevel.None, new LogLevelConfiguration(ConsoleColor.Black));
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new ILoggerServer(name));

    public void Dispose()
    {
        _loggers.Clear();
    }

    public static void LoggerCreate()
    {
        ConsoleLoggerProvider provider = new ConsoleLoggerProvider();
        ILogger logger = provider.CreateLogger("logger");
        logger.LogInformation(
          "This is a test of the emergency broadcast system.");
        logger.LogError("UNE ERREUR EST SURVENUE");

    }
}