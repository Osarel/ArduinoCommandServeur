using Fleck;
using Robot.Action;
using Robot.Event.Args;
using System;
using System.Collections.Generic;

namespace Robot.Event
{
    public class GlobalEvent
    {
        public event EventHandler<ArduinoConnect> ArduinoConnectEvent;
        public event EventHandler<ArduinoDisconnect> ArduinoDisconnectEvent;

        public event EventHandler<RobotPreStart> RobotPreStartEvent;
        public event EventHandler<RobotStarted> RobotStartedEvent;
        public event EventHandler<RobotStopping> RobotStoppingEvent;

        public event EventHandler<SheetStartedEvent> SheetStartedEvent;
        public event EventHandler<SheetFinishEvent> SheetFinishEvent;
        public event EventHandler<ActionStartedEvent> ActionsStartedEvent;
        public event EventHandler<ActionFinishEvent> ActionFinishEvent;

        public event EventHandler<SpeakingStartEvent> SpeakingStartEvent;
        public event EventHandler<SpeakingStopEvent> SpeakingStopEvent;
        public event EventHandler<UserChatEvent> UserChatEvent;

        public event EventHandler<BrowserConnectEvent> BrowserConnectEvent;
        public event EventHandler<BrowserDisconnectEvent> BrowserDisconnectEvent;

        public GlobalEvent()
        {
        }
        
        /*
         * EVENT ROBOT
         */
        public void FireRobotPreStartEvent()
        {
            EventHandlerExtensions.SafeInvoke(RobotPreStartEvent, null, new RobotPreStart());
        }
        public void FireRobotStartedEvent()
        {
            EventHandlerExtensions.SafeInvoke(RobotStartedEvent, null, new RobotStarted());
        }
        public void FireRobotStopping()
        {
            EventHandlerExtensions.SafeInvoke(RobotStoppingEvent, null, new RobotStopping());
        }


        /*
        * EVENT ARDUINO
        */
        public void FireArduinoConnectEvent(Arduino arduino)
        {
            EventHandlerExtensions.SafeInvoke(ArduinoConnectEvent, null, new ArduinoConnect(arduino));
        }
        public void FireArduinoDisconnectEvent(Arduino arduino)
        {
            EventHandlerExtensions.SafeInvoke(ArduinoDisconnectEvent, null, new ArduinoDisconnect(arduino));
        }

        /*
         * EVENT ANIMATION
         */
        public void FireSheetStartedEvent(Sheet Sheet, Dictionary<string, object> Variable)
        {
            EventHandlerExtensions.SafeInvoke(SheetStartedEvent, null, new SheetStartedEvent(Sheet, Variable));
        }
        public void FireSheetFinishEvent(Sheet Sheet)
        {
            EventHandlerExtensions.SafeInvoke(SheetFinishEvent, null, new SheetFinishEvent(Sheet));
        }
        public void FireActionStartedEvent(Sheet Sheet, Liaison Liaison, AbstractAction Action)
        {
            EventHandlerExtensions.SafeInvoke(ActionsStartedEvent, null, new ActionStartedEvent(Sheet, Liaison, Action));
        }
        public void FireActionFinishEvent(Sheet Sheet, AbstractAction Action)
        {
            EventHandlerExtensions.SafeInvoke(ActionFinishEvent, null, new ActionFinishEvent(Sheet, Action));
        }

        /*
        * EVENT CHAT
        */
        public void FireSpeakingStartEvent(string phrase)
        {
            EventHandlerExtensions.SafeInvoke(SpeakingStartEvent, null, new SpeakingStartEvent(phrase));
        }
        public void FireSpeakingStopEvent(string file)
        {
            EventHandlerExtensions.SafeInvoke(SpeakingStopEvent, null, new SpeakingStopEvent(file));
        }
        //TODO
        public void FireUserChatEvent(string phrase)
        {
            EventHandlerExtensions.SafeInvoke(UserChatEvent, null, new UserChatEvent(phrase));
        }

        /*
        * EVENT BROWSER
        */
        //TODO
        public void FireBrowserConnectEvent(IWebSocketConnection connection)
        {
            EventHandlerExtensions.SafeInvoke(BrowserConnectEvent, null, new BrowserConnectEvent(connection));
        }
        //TODO
        public void FireBrowserDisconnectEvent(IWebSocketConnection connection)
        {
            EventHandlerExtensions.SafeInvoke(BrowserDisconnectEvent, null, new BrowserDisconnectEvent(connection));
        }
    }
}
