using System;

namespace Robot.Logger
{
    class LogLevelConfiguration
    {
        public ConsoleColor TitleColor;
        public ConsoleColor ForegroundColor;

        public LogLevelConfiguration(ConsoleColor ForegroundColor, ConsoleColor TitleColor)
        {
            this.ForegroundColor = ForegroundColor;
            this.TitleColor = TitleColor;
        }


    }
}
