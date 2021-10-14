using Fleck;
using System;
namespace Robot.Event.Args
{
    public class BrowserConnectEvent : EventArgs
    {
        public IWebSocketConnection Connection { get; } 

        public BrowserConnectEvent(IWebSocketConnection Connection)
        {
            this.Connection = Connection;
        }
    }
    public class BrowserDisconnectEvent : EventArgs
    {
        public IWebSocketConnection Connection { get; }

        public BrowserDisconnectEvent(IWebSocketConnection Connection)
        {
            this.Connection = Connection;
        }
    }
}
