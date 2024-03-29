﻿using Microsoft.Extensions.Logging;
using Robot.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public sealed class ConsoleLoggerProvider : ILoggerProvider
{
    private Dictionary<LogLevel, LogLevelConfiguration> Config = new Dictionary<LogLevel, LogLevelConfiguration>();
    private readonly ConcurrentDictionary<string, ILoggerServer> _loggers = new ConcurrentDictionary<string, ILoggerServer>();

    public ConsoleLoggerProvider()
    {
        Config.Add(LogLevel.Debug, new LogLevelConfiguration(ConsoleColor.Gray, ConsoleColor.DarkGray));
        Config.Add(LogLevel.Information, new LogLevelConfiguration(ConsoleColor.Magenta, ConsoleColor.DarkMagenta));
        Config.Add(LogLevel.Error, new LogLevelConfiguration(ConsoleColor.Red, ConsoleColor.DarkRed));
        Config.Add(LogLevel.Trace, new LogLevelConfiguration(ConsoleColor.Cyan, ConsoleColor.DarkCyan));
        Config.Add(LogLevel.Warning, new LogLevelConfiguration(ConsoleColor.Yellow, ConsoleColor.DarkYellow));
        Config.Add(LogLevel.None, new LogLevelConfiguration(ConsoleColor.Gray, ConsoleColor.Gray));
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new ILoggerServer(name, Config));

    public void Dispose()
    {
        _loggers.Clear();
    }
}