using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Robot.Serveur
{


    class SocketReply
    {
        private Dictionary<string, object> items = new Dictionary<string, object>();

        public SocketReply(SocketType type, bool error)
        {
            items.Add("type", Enum.GetName(typeof(SocketType), type));
            items.Add("error", error);
        }

        public SocketReply AddErrorMessage(string message)
        {
            items.Add("errorMessage", message);
            return this;
        }
        public SocketReply AddKeyValue(string key, object content)
        {
            items.Add(key, content);
            return this;
        }

        public SocketReply AddContent(object content)
        {
            items.Add("content", content);
            return this;
        }

        public String Build()
        {
            return JsonConvert.SerializeObject(items, Formatting.Indented);
        }

    }
}
