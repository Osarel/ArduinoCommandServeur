using Robot.Action;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Robot
{

    public class ChatIA : UpdatableElement
    {

        public string ID { get; private set; }
        public string Message { get; private set; }
        public int DistanceMAX { get; private set; }
        public string Action { get; private set; }

        public ChatIA(string ID, string Message, int DistanceMAX, string Action)
        {
            this.ID = ID;
            this.Message = Message;
            this.DistanceMAX = DistanceMAX;
            this.Action = Action;
        }

        public static Task<bool> ExecuteChatCommand(string command)
        {
            return Task.Run(() =>
            {
                foreach (KeyValuePair<string, ChatIA> value in ArduinoCommand.robot.Chat)
                { 
                    if (DamerauLevenshteinDistance.Get(command, value.Value.Message) <= value.Value.DistanceMAX)
                    {
                        Sheet sheet = ArduinoCommand.robot.GetSheetByUUID(value.Value.Action);
                        Console.WriteLine("Ici");
                        if (sheet != null)
                        {
                            sheet.StartSheet(null);
                            return true;
                        }
                    }
                }
                return false;
            });
        }

        public bool Stop()
        {
            return true;
        }

        public bool Save()
        {
            ArduinoCommand.robot.SaveChat();
            return true;
        }

        public bool AddToList()
        {
            ArduinoCommand.robot.Chat[ID] = this;
            return true;
        }

        public UpdatableElement GetLastInstance()
        {
            return ArduinoCommand.robot.Chat[ID];
        }
    }
}
