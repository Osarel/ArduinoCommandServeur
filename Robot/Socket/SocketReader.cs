using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Robot.Action;
namespace Robot.Serveur
{

    public class SocketReader
    {
        internal readonly IWebSocketConnection client;
        internal readonly Dictionary<string, object> socketRequest;
        public SocketReplyExecutor Executor { get; set; }
        public SocketReader(IWebSocketConnection client, string json)
        {
            this.client = client;
            socketRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public bool Validate()
        {
            if (!socketRequest.ContainsKey("type") || !Enum.GetNames(typeof(SocketType)).Contains(socketRequest["type"])) {
                return false;
            }

            Executor = getSocketReplyBuilder();
            //SI LA FONCTION NEXISTE PAS CANCEL LA DEMANDE
            if (Executor == null)
            {
                return false;
            }

            //SI L'UTILISATEUR NE CE LOG PAS -> DECONNEXION

            if (!ArduinoCommand.server.connected[client] && !(Executor is ExecutorAuthentification))
            {
                client.Close();
                return false;
            }

            return Executor.Validate();
        }

        public SocketReplyExecutor getSocketReplyBuilder()
        {

            switch (Enum.Parse(typeof(SocketType),(string) socketRequest["type"]))
            {
                case SocketType.AUTHENTIFICATION:
                    return new ExecutorAuthentification(this);
                case SocketType.PORTLISTE:
                    return new ExecutorPortListe(this);
                case SocketType.SYSTEMSTATUS:
                    return new ExecutorSystemStatus(this);
                case SocketType.START:
                    return new ExecutorStartSystem(this);
                case SocketType.STOP:
                    return new ExecutorStopSystem(this);
                case SocketType.RESTART:
                    return new ExecutorRestartSystem(this);
                case SocketType.STOPMOVE:
                    return new ExecutorStopMove(this);
                case SocketType.CREATE:
                    return new ExecutorCreate(this);
                case SocketType.UPDATE:
                    return new ExecutorUpdate(this);
                case SocketType.REMOVE:
                    return new ExecutorRemove(this);
                case SocketType.GET:
                    return new ExecutorGET(this);
                case SocketType.GETLIST:
                    return new ExecutorGETList(this);
                // case SocketType.CONTROL:
                //return new ExecutorControl(this);
                case SocketType.SERVOCONTROL:
                    return new ExecutorServoControl(this);
                case SocketType.LEDCONTROL:
                    return new ExecutorLEDControl(this);
                case SocketType.STARTANIMATION:
                    return new ExecutorStartAnimation(this);

                case SocketType.IACHATGET:
                    return new ExecutorIAChatGet(this);
                case SocketType.IACHATEXECUTE:
                    return new ExecutorIAChatExecute(this);
                default:
                    return null;
            }
        }
    }

    public abstract class SocketReplyExecutor
    {

        protected readonly SocketReader reader;
        protected bool isFinish;


        protected SocketReplyExecutor(SocketReader reader)
        {
            this.reader = reader;
        }

        virtual public bool Validate()
        {
            return true;
        }

        public abstract bool Execute();
    }


     public class ExecutorAuthentification : SocketReplyExecutor
    {

        public ExecutorAuthentification(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("token");
        }

        public override bool Execute()
        {

            //check si le token est le bon (sécurité nul facile à savoir le token en question)
            if (reader.socketRequest["token"].Equals("c62XR2beF2"))
            {
                //Succes envoie d'un socket confirmant la connexion
                ArduinoCommand.server.connected[reader.client] = true;
                reader.client.Send(new SocketReply(SocketType.AUTHENTIFICATION , false).Build());
            } else
            {
                //Echec envoie d'un socket ne confirmant pas la connexion
                //et deconnexion
                Console.WriteLine("Deconnexion de l'utilisateur car token incorect");
                reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("Identifiant incorrect").Build());
                reader.client.Close();
            }
            return true;
        }
    }

    public class ExecutorPortListe : SocketReplyExecutor
    {

        public ExecutorPortListe(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            reader.client.Send(new SocketReply(SocketType.PORTLISTE, false).AddContent(Arduino.SerialPorts()).Build());
            return true;
        }
    }

    public class ExecutorSystemStatus : SocketReplyExecutor
    {

        public ExecutorSystemStatus(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            reader.client.Send(new SocketReply(SocketType.SYSTEMSTATUS, false).AddContent(ArduinoCommand.robot.StatusSystem()).Build());
            return true;
        }
    }

    public class ExecutorStartSystem : SocketReplyExecutor
    {

        public ExecutorStartSystem(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            ArduinoCommand.robot.StartRobot();
            reader.client.Send(new SocketReply(SocketType.START, false).Build());
            return true;
        }
    }

    public class ExecutorStopSystem : SocketReplyExecutor
    {

        public ExecutorStopSystem(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            ArduinoCommand.demande_arret = true;
            reader.client.Send(new SocketReply(SocketType.STOP, false).Build());
            return true;
        }
    }
    public class ExecutorRestartSystem : SocketReplyExecutor
    {

        public ExecutorRestartSystem(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            ArduinoCommand.demande_restart = true;
            Console.WriteLine("Demande restart");
            reader.client.Send(new SocketReply(SocketType.RESTART, false).Build());
            return true;
        }
    }

    public class ExecutorStopMove : SocketReplyExecutor
    {

        public ExecutorStopMove(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            ArduinoCommand.robot.StopMoveRobot();
            reader.client.Send(new SocketReply(SocketType.STOPMOVE, false).Build());
            return true;
        }
    }

    public class ExecutorServoControl : SocketReplyExecutor
    {


        public ExecutorServoControl(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            if (!reader.socketRequest.ContainsKey("id") && !reader.socketRequest.ContainsKey("position"))
            {
                return false;
            }
            return true;
            
        }

        public override bool Execute()
        {
            try
            {
                IDictionary<string, Element> elements = ArduinoCommand.robot.Elements;
                string id = (string)reader.socketRequest["id"];
                int position = (int)(long) reader.socketRequest["position"];
                if (!elements.ContainsKey(id))
                {
                    reader.client.Send(new SocketReply(SocketType.SERVOCONTROL, true).AddErrorMessage("ERREUR PIN INCORRECT").Build());
                    return false;
                }
                ServoMotor servo = elements[id] as ServoMotor;
                if (servo == null)
                {
                    reader.client.Send(new SocketReply(SocketType.SERVOCONTROL, true).AddErrorMessage("ERREUR PIN INCORRECT").Build());
                    return false;
                }
                if (position < 0 || position > 100)
                {
                    reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("position non valide").Build());
                    return false;
                }
                servo.SendPosition(position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }
    }
    public class ExecutorLEDControl : SocketReplyExecutor
    {


        public ExecutorLEDControl(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            if (!reader.socketRequest.ContainsKey("id") && !reader.socketRequest.ContainsKey("r") && !reader.socketRequest.ContainsKey("g") && !reader.socketRequest.ContainsKey("b") && !reader.socketRequest.ContainsKey("a"))
            {
                return false;
            }
            return true;

        }

        public override bool Execute()
        {
            try
            {
                IDictionary<string, Element> elements = ArduinoCommand.robot.Elements;
                string id = (string)reader.socketRequest["id"];
                int r = (int)(long)reader.socketRequest["r"];
                int g = (int)(long)reader.socketRequest["g"];
                int b = (int)(long)reader.socketRequest["b"];
                int a = (int)(long)reader.socketRequest["a"];
                if (!elements.ContainsKey(id))
                {
                    reader.client.Send(new SocketReply(SocketType.SERVOCONTROL, true).AddErrorMessage("ERREUR PIN INCORRECT").Build());
                    return false;
                }
                LED led = elements[id] as LED;
                if (led == null)
                {
                    reader.client.Send(new SocketReply(SocketType.SERVOCONTROL, true).AddErrorMessage("ERREUR PIN INCORRECT").Build());
                    return false;
                }
                if (r < 0 || r > 255 || b < 0 || b > 255 || g < 0 || g > 255 || a <0 || a > 255 )
                {
                    reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("position non valide").Build());
                    return false;
                }
                led.SendColorToAll(new Color(r, g, b));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }
    }
    public class ExecutorUpdate : SocketReplyExecutor
    {

        public ExecutorUpdate(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("element") && reader.socketRequest.ContainsKey("data");
        }

        public override bool Execute()
        {
            try
            {
                JObject obj = (JObject)reader.socketRequest["element"];
                UpdatableElement e;
                switch ((string)reader.socketRequest["data"])
                {
                    case "element":
                        e = obj.ToObject<Element>();
                        break;
                    case "sheet":
                        e = obj.ToObject<Sheet>();
                        break;
                    case "arduino":
                        e = obj.ToObject<Arduino>();
                        break;
                    case "chat":
                        e = obj.ToObject<ChatIA>();
                        break;
                    case "config":
                        e = obj.ToObject<RobotOption>();
                        break;
                    default:
                        return false;
                }
                UpdatableElement laste = e.GetLastInstance();
                if (laste != null)
                {
                    laste.Stop();
                }
                e.AddToList();
                e.Save();
                reader.client.Send(new SocketReply(SocketType.UPDATE, false).AddContent(e).Build());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("Erreur dans la lecture").Build());
                return false;
            }
        }
    }

    public class ExecutorCreate : SocketReplyExecutor
    {

        public ExecutorCreate(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("data");
        }

        public override bool Execute()
        {
            try
            {

                UpdatableElement e;
                switch ((string)reader.socketRequest["data"])
                {
                    case "element":
                        if (!reader.socketRequest.ContainsKey("elementType"))
                        {
                            return false;
                        }
                        e = Element.CreateInstanceOf((ElementType) Enum.Parse(typeof(ElementType), (string) reader.socketRequest["elementType"]));
                        break;
                    case "sheet":
                        e = new Sheet(Guid.NewGuid().ToString(), "No name", new Dictionary<string, AbstractAction>(), new Dictionary<string, object>(), new Liaison[0]);
                        break;
                    case "arduino":
                        e = new Arduino("no name", Guid.NewGuid().ToString(), "No port", 9600);
                        break;
                    case "chat":
                        e = new ChatIA(Guid.NewGuid().ToString(), "", 3, "no action");
                        break;
                    default:
                        return false;
                }
                e.AddToList();
                e.Save();
                return new ExecutorGETList(reader).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("Erreur dans la lecture").Build());
                return false;
            }
        }
    }

    public class ExecutorGETList: SocketReplyExecutor
    {

        public ExecutorGETList(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("data");
        }

        public override bool Execute()
        {
            string Type = (string)reader.socketRequest["data"];
            SocketReply reply = new SocketReply(SocketType.GETLIST, false).AddKeyValue("data", Type); 
                switch (Type)
                {
                    case "element":
                       reply.AddContent(ArduinoCommand.robot.GetElementsNames());
                       reader.client.Send(new SocketReply(SocketType.ELEMENTSINFO, false).AddContent(ArduinoCommand.robot.GetElementsInfo()).Build());
                    break;
                    case "sheet":
                    reply.AddContent(ArduinoCommand.robot.GetAnimationsNames());
                    break;
                    case "arduino":
                    reply.AddContent(ArduinoCommand.robot.GetArduinoNames());
                    break;
                    case "chat":
                    reply.AddContent(ArduinoCommand.robot.GetChatNames());
                    break;
                default:
                        return false;
                }
            reader.client.Send(reply.Build());
            return true;
        }
    }

    public class ExecutorGET : SocketReplyExecutor
    {

        public ExecutorGET(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("data") && reader.socketRequest.ContainsKey("id");
        }

        public override bool Execute()
        {
            string Type = (string)reader.socketRequest["data"];
            string id = (string) reader.socketRequest["id"];
            SocketReply reply = new SocketReply(SocketType.GET, false).AddKeyValue("data", Type);
            switch (Type)
            {
                case "element":
                    reply.AddContent(ArduinoCommand.robot.GetElementByUUID(id));
                    break;
                case "sheet":
                    reply.AddContent(ArduinoCommand.robot.GetSheetByUUID(id));
                    break;
                case "arduino":
                    reply.AddContent(ArduinoCommand.robot.GetArduinoByUUID(id));
                    break;
                case "chat":
                    reply.AddContent(ArduinoCommand.robot.Chat[id]);
                    break;
                case "config":
                    reply.AddContent(ArduinoCommand.robot.Option);
                    break;
                default:
                    return false;
            }
            reader.client.Send(reply.Build());
            return true;
        }
    }
    public class ExecutorRemove : SocketReplyExecutor
    {

        public ExecutorRemove(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("data") && reader.socketRequest.ContainsKey("id");
        }

        public override bool Execute()
        {
            string type = (string)reader.socketRequest["data"];
            string id = (string)reader.socketRequest["id"];
            ArduinoCommand.robot.RemoveUpdatableComponent(type, id);
            reader.client.Send(new SocketReply(SocketType.REMOVE, false).Build());
            return true;
        }
    }

    public class ExecutorStartAnimation : SocketReplyExecutor
    {

        public ExecutorStartAnimation(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("id");
        }

        public override bool Execute()
        {
            string id = (string)reader.socketRequest["id"];
            Sheet e = ArduinoCommand.robot.GetAnimations()[id];
            if (e != null)
            {
                ArduinoCommand.robot.GetAnimations()[id].StartSheet(null);
                reader.client.Send(new SocketReply(SocketType.STARTANIMATION, false).AddContent(id).Build());
                return true;
            }
            reader.client.Send(new SocketReply(SocketType.ERROR, true).AddErrorMessage("L'animation n'existe déjà pas").Build());
            return false;
        }
    }




    public class ExecutorIAChatGet : SocketReplyExecutor
    {

        public ExecutorIAChatGet(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return true;
        }

        public override bool Execute()
        {
            reader.client.Send(new SocketReply(SocketType.IACHATGET, false).AddContent(ArduinoCommand.robot.Chat).Build());
            return true;
        }
    }

    public class ExecutorIAChatExecute : SocketReplyExecutor
    {

        public ExecutorIAChatExecute(SocketReader reader) : base(reader)
        {
        }


        override public bool Validate()
        {
            return reader.socketRequest.ContainsKey("chat");
        }

        public override bool Execute()
        {
            string chat = (string)reader.socketRequest["chat"];
            Task<bool> s = ChatIA.ExecuteChatCommand(chat);
            s.Wait();
            if (s.Result)
            {
                reader.client.Send(new SocketReply(SocketType.IACHATEXECUTE, true).AddErrorMessage("Action effectuer avec succès").Build());
                return true;
            } else
            {
                reader.client.Send(new SocketReply(SocketType.IACHATEXECUTE, true).AddErrorMessage("L'action n'existe pas").Build());
                return true;
            }
        }
    }


}
