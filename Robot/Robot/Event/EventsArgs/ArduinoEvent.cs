using System;
namespace Robot.Event.Args
{
    public class ArduinoConnect : EventArgs
    {
        public Arduino Arduino { get; }

        public ArduinoConnect(Arduino Arduino)
        {
            this.Arduino = Arduino;
        }
    }

    public class ArduinoDisconnect : EventArgs
    {
        public Arduino Arduino { get; }

        public ArduinoDisconnect(Arduino Arduino)
        {
            this.Arduino = Arduino;
        }
    }
}
