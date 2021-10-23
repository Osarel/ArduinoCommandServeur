using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AnaloguePINAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Element;
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly float Value;
        [JsonConstructor]
        public AnaloguePINAction(string Element, float Value, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.ANALOG, false, ID, Position, Output)
        {
            this.Element = Element;
            this.Value = Value;
        }

        protected override void Launch(Liaison caller)
        {
            if (!(ArduinoCommand.robot.GetElementByUUID(Element) is PIN pin))
            {
                return;
            }
            if (Value < 0)
            {
                return;
            }
            if (pin.DigitalType != PINType.ANALOG_INPUT)
            {
                pin.WriteValue(Value);
            }
        }
    }


}
