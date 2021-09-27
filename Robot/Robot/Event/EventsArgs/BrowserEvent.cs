using Fleck;
using System;
namespace Robot.Event.Args
{
    class BrowserConnectEvent : EventArgs
    {
        public IWebSocketConnection Connection { get; } 

        public BrowserConnectEvent(IWebSocketConnection Connection)
        {
            this.Connection = Connection;
        }
    }
    class BrowserDisconnectEvent : EventArgs
    {
        public IWebSocketConnection Connection { get; }

        public BrowserDisconnectEvent(IWebSocketConnection Connection)
        {
            this.Connection = Connection;
        }
    }
}
