using System;
namespace Robot.Event.Args
{
    class ArduinoConnect : EventArgs
    {
        public Arduino Arduino { get; } 

        public ArduinoConnect(Arduino Arduino)
        {
            this.Arduino = Arduino;
        }
    }

    class ArduinoDisconnect : EventArgs
    {
        public Arduino Arduino { get; }

        public ArduinoDisconnect(Arduino Arduino)
        {
            this.Arduino = Arduino;
        }
    }
}
