using System;

namespace Robot.Event.Args
{
    public class RobotPreStart : EventArgs
    {
        public RobotPreStart()
        {
        }
    }
    public class RobotStarted : EventArgs
    {
        public RobotStarted()
        {
        }
    }
    public class RobotStopping : EventArgs
    {
        public RobotStopping()
        {
        }
    }
}
