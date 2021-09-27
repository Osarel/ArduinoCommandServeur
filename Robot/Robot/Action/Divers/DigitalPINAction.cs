using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DigitalPINAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Element;
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly DigitalState Value;
        [JsonConstructor]
        public DigitalPINAction(string Element, DigitalState Value, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.DIGITAL, false, ID, Position, Output)
        {
            this.Element = Element;
            this.Value = Value;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            PIN pin = ArduinoCommand.robot.GetElementByUUID(Element) as PIN;
            if (pin == null)
            {
                return;
            }
            if (pin.DigitalType != PINType.DIGITAL_OUTPUT)
            {
                pin.WriteValue(Value);
            }
        }
    }


}
    