using System;

namespace Robot.Event
{
    class GlobalEvent
    {
        public event EventHandler<Args.ArduinoConnect> ArduinoConnectEvent;
        public event EventHandler<Args.ArduinoDisconnect> ArduinoDisconnectEvent;

        public event EventHandler<Args.RobotPreStart> RobotPreStartEvent;
        public event EventHandler<Args.RobotStarted> RobotStartedEvent;
        public event EventHandler<Args.RobotStopping> RobotStoppingEvent;

        public event EventHandler<Args.SheetStartedEvent> SheetStartedEvent;
        public event EventHandler<Args.SheetFinishEvent> SheetFinishEvent;
        public event EventHandler<Args.ActionStartedEvent> ActionsStartedEvent;
        public event EventHandler<Args.ActionFinishEvent> ActionFinishEvent;

        public event EventHandler<Args.SpeakingStartEvent> SpeakingStartEvent;
        public event EventHandler<Args.SpeakingStopEvent> SpeakingStopEvent;
        public event EventHandler<Args.UserChatEvent> UserChatEvent;

        public event EventHandler<Args.BrowserConnectEvent> BrowserConnectEvent;
        public event EventHandler<Args.BrowserDisconnectEvent> BrowserDisconnectEvent;
    }
}
