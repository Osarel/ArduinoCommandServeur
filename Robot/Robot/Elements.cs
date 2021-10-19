using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Robot.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Robot
{

    public enum ElementType
    {

        SERVOMOTOR,
        LED,
        PIN,

    }

    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(ElementConverter))]

    public abstract class Element : IUpdatableElement
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int PIN { get; set; }
        [JsonProperty]
        public float ActualValue;
        [JsonProperty]
        public string ID { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType Type { get; set; }
        [JsonProperty]
        public string Position { get; set; }

        [JsonProperty]
        public string RequestArduino { get; set; }

        private Arduino arduino;
        public event EventHandler<Event.Args.ElementActualValueChanged> ActualValueChangedHandler;

        protected Element(string ID, string Name, int PIN, ElementType Type, string Position, string RequestArduino)
        {
            this.Name = Name;
            this.ID = ID;
            this.PIN = PIN;
            this.Type = Type;
            this.Position = Position;
            this.RequestArduino = RequestArduino;
        }

        public Arduino GetArduino()
        {
            if (arduino == null)
            {
                if (!ArduinoCommand.robot.Arduinos.ContainsKey(RequestArduino))
                {
                    ArduinoCommand.log.LogError("erreur selection arduino {0} : {1}", RequestArduino, ID);
                    return null;
                }
                arduino = ArduinoCommand.robot.Arduinos[RequestArduino];
            }

            return arduino;
        }

        public virtual void StopElementAction()
        {

        }

        public virtual void InitialiseElement()
        {

        }
        public float GetActualValue()
        {
            return ActualValue;
        }
        public void SetActualValue(float value)
        {
            ActualValue = value;
            EventHandlerExtensions.SafeInvoke(ActualValueChangedHandler, this, new Event.Args.ElementActualValueChanged(value));
        }

        public static Element CreateInstanceOf(ElementType v)
        {
            return v switch
            {
                ElementType.LED => new LED(Guid.NewGuid().ToString(), "No name", 0, "NONE", "no arduino", 10, 150),
                ElementType.PIN => new PIN(Guid.NewGuid().ToString(), "No name", 0, "NONE", "no arduino", PINType.DIGITAL_OUTPUT),
                ElementType.SERVOMOTOR => new ServoMotor(Guid.NewGuid().ToString(), "No name", 0, "NONE", "no arduino", 0, 180, 90, 2000, 1000),
                _ => null,
            };
        }

        public bool Stop()
        {
            StopElementAction();
            return true;
        }

        public bool Save()
        {
            ArduinoCommand.robot.SaveElements();
            return true;
        }

        public bool AddToList()
        {
            ArduinoCommand.robot.Elements[ID] = this;
            return true;
        }

        public IUpdatableElement GetLastInstance()
        {
            return ArduinoCommand.robot.GetElementByUUID(ID);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ServoMotor : Element
    {
        [JsonProperty]
        public int Maximum { get; set; }
        [JsonProperty]
        public int Defaut { get; set; }
        [JsonProperty]
        public int Minimum { get; set; }
        [JsonProperty]
        public int Timeout { get; set; }
        [JsonProperty]
        public int Temps { get; set; }
        public int ActiveTimer { get; set; }

        private Timer timer = new Timer(1000);
        private bool active;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                ActiveTimer = Timeout;
                if (!active && value)
                {
                    timer.Enabled = true;
                }
                active = value;
            }
        }

        public void CountDownActive(Object source, ElapsedEventArgs e)
        {
            ActiveTimer -= 1;
            if (ActiveTimer <= 0)
            {
                Active = false;
                timer.Stop();
                ServoDetach();
            }
        }

        public override void StopElementAction()
        {
            ActiveTimer = 0;
        }

        [JsonConstructor]
        public ServoMotor(string id, string name, int PIN, string Position, string arduino, int Minimum, int Maximum, int Defaut, int Timeout, int Temps) : base(id, name, PIN, ElementType.SERVOMOTOR, Position, arduino)
        {
            this.Maximum = Maximum;
            this.Defaut = Defaut;
            this.Minimum = Minimum;
            this.Timeout = Timeout;
            this.Temps = Temps;
            timer = new Timer(1000);
            timer.Elapsed += CountDownActive;
            timer.AutoReset = true;
        }

        public int PourcentFromPosition(int position)
        {
            return ((position - Minimum) * 100) / (Maximum - Minimum);
        }

        public int PositionFromPourcent(int percent)
        {
            return ((percent * (Maximum - Minimum) / 100) + Minimum);
        }

        public int CalculateTimeToPosition(int position)
        {
            return CalculateTimeFromPositionToPosition((int)ActualValue, position);
        }
        public int CalculateTimeFromPositionToPosition(int fromPosition, int toPosition)
        {
            int AngleTotal = Maximum - Minimum;
            int AngleGoal = fromPosition >= toPosition ? fromPosition - toPosition : toPosition - fromPosition;
            if (AngleGoal <= 0)
            {
                return 0;
            }
            return Temps / AngleTotal * AngleGoal;
        }


        public void SendPosition(int value)
        {
            if (value < 0 || value > 100)
            {
                return;
            }

            int position = PositionFromPourcent(value);
            string commande = new StringBuilder("rob://a/").Append(PIN).Append("/").Append(position).Append("/").ToString();
            if (!GetArduino().Write(commande))
            {
                ArduinoCommand.log.LogDebug("Echec de l'envoie de la commande à l'arduino");
            }
            SetActualValue(value);
            Active = true;

        }

        public void ServoDetach()
        {
            GetArduino().Write("rob://b/" + PIN + "/");
        }

        public void ResetPosition()
        {
            SendPosition(Defaut);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class LED : Element
    {
        [JsonProperty]
        public int NombreLED { get; set; }
        [JsonProperty]
        public int Brightness { get; set; }

        [JsonConstructor]
        public LED(string id, string name, int PIN, string Position, string arduino, int NombreLED, int Brightness) : base(id, name, PIN, ElementType.LED, Position, arduino)
        {
            this.NombreLED = NombreLED;
            this.Brightness = Brightness;
        }

        public override void InitialiseElement()
        {
            string message = new StringBuilder("rob://c/").Append(PIN).Append("/").Append(NombreLED).Append("/").Append(Brightness).Append("/").ToString();
            Arduino arduino = GetArduino();
            if (arduino == null)
            {
                return;
            }
            arduino.Write(message);
        }


        public void SendPixelColor(int led, Color color, bool show)
        {
            string message = new StringBuilder("rob://e/")
                    .Append(PIN).Append("/")
                    .Append(led).Append("/")
                    .Append(color.R).Append("/")
                    .Append(color.G).Append("/")
                    .Append(color.B).Append("/")
                    .Append(show ? 1 : 0).Append("/")
                    .ToString();
            Arduino arduino = GetArduino();
            arduino.Write(message);
        }

        public void Update()
        {
            string message = new StringBuilder("rob://f/")
                    .Append(PIN).Append("/")
                    .ToString();
            Arduino arduino = GetArduino();
            arduino.Write(message);
        }

        public void Clear()
        {
            string message = new StringBuilder("rob://g/")
                    .Append(PIN).Append("/")
                    .ToString();
            Arduino arduino = GetArduino();
            arduino.Write(message);
        }

        public void SendColorToAll(Color color)
        {
            Arduino arduino = GetArduino();
            string message = new StringBuilder("rob://d/")
                .Append(PIN).Append("/")
                .Append(color.R).Append("/")
                .Append(color.G).Append("/")
                .Append(color.B).Append("/")
                .ToString();
            arduino.Write(message);
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class PIN : Element
    {

        public bool Updated = false;
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public PINType DigitalType { get; private set; }
        [JsonConstructor]
        public PIN(string ID, string Name, int PIN, string Position, string RequestArduino, PINType DigitalType) : base(ID, Name, PIN, ElementType.PIN, Position, RequestArduino)
        {
            this.DigitalType = DigitalType;
        }

        public override void StopElementAction()
        {
            GetArduino().PinAction.Remove(PIN);
        }

        public override void InitialiseElement()
        {
            if (DigitalType == PINType.DIGITAL_INPUT)
            {
                string message = new StringBuilder("rob://i/")
                    .Append(PIN).Append("/")
                    .ToString();
                Arduino arduino = GetArduino();
                Action<Dictionary<string, object>, object> a = new Action<Dictionary<string, object>, object>(UpdateCurrentValue);
                arduino.AddAction(PIN, new Arduino.ThreadActionPin(a, null));
                arduino.Write(message);
            }
        }
        public void UpdateCurrentValue(Dictionary<string, object> request, object n)
        {
            SetActualValue((int)(long)request["state"]);
        }

        public void RequestAnalogValue()
        {
            string message = new StringBuilder("rob://m/")
            .Append(PIN).Append("/")
            .ToString();
            Arduino arduino = GetArduino();
            arduino.Write(message);
        }
        public void WriteValue(DigitalState value)
        {
            WriteValue(value == DigitalState.LOW ? 0 : 1);
        }

        public void WriteValue(float value)
        {
            string message;
            switch (DigitalType)
            {
                case PINType.DIGITAL_OUTPUT:
                    message = new StringBuilder("rob://")
                 .Append(value == 0 ? "k" : "j")
                 .Append("/")
                 .Append(PIN).Append("/")
                 .ToString();
                    break;
                case PINType.ANALOG_OUPUT:
                    message = new StringBuilder("rob://l/")
                    .Append(PIN).Append("/")
                    .Append(value).Append("/")
                    .ToString();
                    break;
                default:
                    return;
            }
            Arduino arduino = GetArduino();
            arduino.Write(message);
        }
    }

    public enum PINType
    {
        ANALOG_INPUT,
        ANALOG_OUPUT,
        DIGITAL_INPUT,
        DIGITAL_OUTPUT
    }
    public enum DigitalState
    {
        HIGH = 1,
        LOW = 0,
    }
}
