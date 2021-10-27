using Newtonsoft.Json;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ServoAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Element;
        [JsonProperty]
        private readonly int PositionServo;
        [JsonConstructor]
        public ServoAction(string Element, int PositionServo, string ID, CubePositionAction Cube) : base(ActionType.SERVO, false, ID, Cube)
        {
            this.Element = Element;
            this.PositionServo = PositionServo;
        }

        protected override void Launch()
        {
            if (!(ArduinoCommand.robot.GetElementByUUID(Element) is ServoMotor servo))
            {
                return;
            }
            int estimateTime = servo.CalculateTimeToPosition(PositionServo);
            servo.SendPosition(PositionServo);
            Thread.Sleep(estimateTime);
        }
    }
}
