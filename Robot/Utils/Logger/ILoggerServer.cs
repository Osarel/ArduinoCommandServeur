using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
namespace Robot.Logger
{
    class ILoggerServer : ILogger
    {
        private readonly string _name;
        private readonly Dictionary<LogLevel, LogLevelConfiguration> Config;

        public ILoggerServer(string name, Dictionary<LogLevel, LogLevelConfiguration> Config)
        {
            this._name = name;
            this.Config = Config;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
            {
                return ArduinoCommand.robot.Options.debug;
            }
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");
            Console.ForegroundColor = Config[logLevel].TitleColor;
            Console.Write($"{_name} : {logLevel}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]");
            Console.ForegroundColor = Config[logLevel].ForegroundColor;
            Console.WriteLine($" {formatter(state, exception)}");
            Console.ForegroundColor = originalColor;
        }
    }
}
