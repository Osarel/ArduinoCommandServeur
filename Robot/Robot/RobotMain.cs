using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robot.Action;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Robot
{

    public class RobotMain
    {
        public IDictionary<string, Element> Elements { get; private set; }

        public IDictionary<string, Arduino> Arduinos { get; private set; }

        public IDictionary<string, Sheet> Animations { get; private set; }
        public IDictionary<string, ChatIA> Chat { get; private set; }
        public RobotOption Options { get; set; }

        public RobotMain()
        {
            Elements = new Dictionary<string, Element>();
            Arduinos = new Dictionary<string, Arduino>();
            Animations = new Dictionary<string, Sheet>();
            Options = new RobotOption();
            Chat = new Dictionary<string, ChatIA>();
        }

        public IDictionary<string, string> GetChatNames()
        {
            IDictionary<string, string> list = new Dictionary<string, string>();
            foreach (ChatIA e in Chat.Values)
            {
                list.Add(e.ID, e.Message);
            }
            return list;
        }

        public Arduino GetArduinoByUUID(string id)
        {
            return Arduinos.ContainsKey(id) ? Arduinos[id] : null;
        }
        public IDictionary<string, string> GetArduinoNames()
        {
            IDictionary<string, string> list = new Dictionary<string, string>();
            foreach (Arduino e in Arduinos.Values)
            {
                list.Add(e.ID, e.Name);
            }
            return list;
        }
        public Element GetElementByUUID(string id)
        {
            return Elements.ContainsKey(id) ? Elements[id] : null;
        }


        public Sheet GetSheetByUUID(string id)
        {
            return Animations.ContainsKey(id) ? Animations[id] : null;
        }


        public IDictionary<string, Element> GetElements()
        {
            return Elements;
        }
        public IDictionary<string, string> GetElementsNames()
        {
            IDictionary<string, string> list = new Dictionary<string, string>();
            foreach (Element e in Elements.Values)
            {
                list.Add(e.ID, e.Name);
            }
            return list;
        }

        public IDictionary<string, Sheet> GetAnimations()
        {
            return Animations;
        }

        public IDictionary<string, string> GetAnimationsNames()
        {
            IDictionary<string, string> list = new Dictionary<string, string>();
            foreach (Sheet e in Animations.Values)
            {
                list.Add(e.ID, e.Name);
            }
            return list;
        }

        public bool RemoveUpdatableComponent(string type, string id)
        {
            switch (type)
            {
                case "element":
                    ArduinoCommand.robot.Elements.Remove(id);
                    DeleteFileStorage("Elements", ".elem", id);
                    break;
                case "sheet":
                    ArduinoCommand.robot.Animations.Remove(id);
                    DeleteFileStorage("Animations", ".anim", id);
                    break;
                case "arduino":
                    ArduinoCommand.robot.Arduinos.Remove(id);
                    DeleteFileStorage("Arduinos", ".ard", id);
                    break;
                case "chat":
                    ArduinoCommand.robot.Chat.Remove(id);
                    break;
                default:
                    return false;
            }
            return false;
        }

        private void DeleteFileStorage(string type, string extentions, string id)
        {
            string path = new StringBuilder(Directory.GetCurrentDirectory())
                .Append("/")
                .Append(type)
                .Append("/")
                .Append(id)
                .Append(extentions).ToString();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        public void LoadDataFromConfig()
        {
            LoadElements();
            LoadAnimations();
            LoadArduino();
            LoadConfig();
            LoadChat();
        }

        public void SaveDataToConfig()
        {
            SaveAnimations();
            SaveArduino();
            SaveConfig();
            SaveChat();
            SaveElements();
        }

        public void LoadAnimations()
        {
            Console.WriteLine("Chargement des animations ..");
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Animations/");
            foreach (string value in files)
            {
                try
                {
                    Console.WriteLine("Animation : " + value);
                    using StreamReader file = File.OpenText(value);
                    JsonSerializer serializer = new JsonSerializer();
                    Sheet sheet = (Sheet)serializer.Deserialize(file, typeof(Sheet));
                    Animations.Add(sheet.ID, sheet);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la récuperation de l'animation : " + value);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void SaveAnimations()
        {
            Console.WriteLine("Sauvegarde des animations ..");
            string folder = Directory.GetCurrentDirectory() + "/Animations/";
            foreach (Sheet value in Animations.Values)
            {
                try
                {
                    string filePath = folder + value.ID + ".anim";
                    Console.WriteLine("Animation save : " + value);
                    using StreamWriter file = File.CreateText(filePath);
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la sauvegarde de l'animation : " + value);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void LoadArduino()
        {
            Console.WriteLine("Chargement des arduinos ..");
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Arduinos/");
            foreach (string value in files)
            {
                try
                {
                    Console.WriteLine("Arduino : " + value);
                    using StreamReader file = File.OpenText(value);
                    JsonSerializer serializer = new JsonSerializer();
                    Arduino arduino = (Arduino)serializer.Deserialize(file, typeof(Arduino));
                    Arduinos[arduino.ID] = arduino;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la récuperation de l'arduino : " + value);
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void SaveArduino()
        {
            Console.WriteLine("Sauvegarde des arduinos ..");
            string folder = Directory.GetCurrentDirectory() + "/Arduinos/";
            foreach (Arduino value in Arduinos.Values)
            {
                try
                {
                    string filePath = folder + value.ID + ".ard";
                    Console.WriteLine("Arduino save : " + value);
                    using StreamWriter file = File.CreateText(filePath);
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la sauvegarde de l'animation : " + value);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void LoadElements()
        {
            Console.WriteLine("Chargement des elements ..");
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Elements/");
            foreach (string value in files)
            {
                try
                {
                    Console.WriteLine("Element : " + value);
                    using StreamReader file = File.OpenText(value);
                    JsonSerializer serializer = new JsonSerializer();
                    Element element = (Element)serializer.Deserialize(file, typeof(Element));
                    Elements.Add(element.ID, element);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la récuperation de : " + value);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void SaveElements()
        {
            Console.WriteLine("Sauvegarde des elements ..");
            string folder = Directory.GetCurrentDirectory() + "/Elements/";
            foreach (Element value in Elements.Values)
            {
                try
                {
                    string filePath = folder + value.ID + ".elem";
                    Console.WriteLine("Element save : " + value);
                    using StreamWriter file = File.CreateText(filePath);
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Echec dans la sauvegarde de l'element : " + value);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void LoadConfig()
        {
            Console.WriteLine("Chargement des fichiers de configurations ..");
            string filePath = Directory.GetCurrentDirectory() + "/config.json";
            if (File.Exists(filePath))
            {
                using StreamReader file = File.OpenText(filePath);
                JsonSerializer serializer = new JsonSerializer();
                Console.WriteLine("Dossier trouver en cours de  récuperation.");
                Options = (RobotOption)serializer.Deserialize(file, typeof(RobotOption));
                return;
            }
            Console.WriteLine("Dossier introuvable creation d'une nouvelle configuration.");
            Options = new RobotOption();
        }

        public void SaveConfig()
        {
            string filePath = Directory.GetCurrentDirectory() + "/config.json";
            using StreamWriter file = File.CreateText(filePath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, Options);
        }

        public void SaveChat()
        {
            string filePath2 = Directory.GetCurrentDirectory() + "/ChatIA.json";
            using StreamWriter file2 = File.CreateText(filePath2);
            JsonSerializer serializer2 = new JsonSerializer();
            serializer2.Serialize(file2, Chat);
        }

        public void LoadChat()
        {
            Console.WriteLine("Chargement des fichiers de chat ..");
            string filePath = Directory.GetCurrentDirectory() + "/ChatIA.json";
            if (File.Exists(filePath))
            {
                using StreamReader file = File.OpenText(filePath);
                JsonSerializer serializer = new JsonSerializer();
                Console.WriteLine("Dossier trouver en cours de  récuperation.");
                Chat = (Dictionary<string, ChatIA>)serializer.Deserialize(file, typeof(Dictionary<string, ChatIA>));
            }
        }
        internal Dictionary<string, ArduinoStatus> StatusSystem()
        {
            Dictionary<string, ArduinoStatus> dict = new Dictionary<string, ArduinoStatus>();
            foreach (KeyValuePair<string, Arduino> entry in Arduinos)
            {
                dict[entry.Key] = entry.Value.GetArduinoStatus();
            }
            return dict;
        }

        public void StartRobot()
        {
            foreach (KeyValuePair<string, Arduino> entry in Arduinos)
            {
                entry.Value.StartCommunication();
            }
            Thread.Sleep(5000);
            foreach (KeyValuePair<string, Element> entry in Elements)
            {
                entry.Value.InitialiseElement();
            }
            if (Options.startingAnimator != null)
            {
                Sheet e = ArduinoCommand.robot.GetAnimations()[Options.startingAnimator];
                if (e != null)
                {
                    e.StartSheet(null);
                }
            }

        }

        public void StopRobot()
        {
            StopMoveRobot();
            foreach (KeyValuePair<string, Arduino> entry in Arduinos)
            {
                entry.Value.Stop();
            }
        }

        public void StopMoveRobot()
        {
            foreach (KeyValuePair<string, Sheet> entry in Animations)
            {
                entry.Value.ForceStopSheet();
            }
            foreach (KeyValuePair<string, Element> entry in Elements)
            {
                switch (entry.Value.Type)
                {
                    case ElementType.SERVOMOTOR:
                        (entry.Value as ServoMotor).ServoDetach();
                        break;
                    case ElementType.LED:
                        (entry.Value as LED).SendColorToAll(Color.BLACK);
                        break;
                }
            }
        }

        public Dictionary<string, JObject> GetElementsInfo()
        {
            Dictionary<string, JObject> result = new Dictionary<string, JObject>();
            foreach (Element value in Elements.Values)
            {
                JObject o = new JObject
                {
                    { "type", Enum.GetName(typeof(ElementType), value.Type) },
                    { "position", value.Position }
                };
                result.Add(value.ID, o);
            }
            return result;
        }
    }
}
