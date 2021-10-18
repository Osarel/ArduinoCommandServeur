using System;
using Microsoft.Extensions.Logging;
namespace Robot.Logger
{
    class ILoggerServer : ILogger 
    {
        private readonly string _name;

        public ILoggerServer(string name)
        {
            this._name = name;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) =>
            true;

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

            Console.ForegroundColor = ConsoleColor.Green ;
            Console.Write($"[{logLevel,-12}]");
            Console.WriteLine($"{formatter(state, exception)}");
            Console.ForegroundColor = originalColor;
        }
    }
}
