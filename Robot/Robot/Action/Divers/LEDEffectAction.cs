using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LEDEffectAction : AbstractAction
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly LEDEffectType LEDEffectType;
        [JsonProperty]
        private readonly Color BaseColor;
        [JsonProperty]
        private readonly string Element;
        [JsonProperty]
        private readonly int Timeout;
        [JsonProperty]
        private readonly int Speed;
        [JsonConstructor]
        public LEDEffectAction(bool Async, LEDEffectType LEDEffectType, Color BaseColor, string Element, int Timeout, int Speed, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.LED, Async, ID, Position, Output)
        {
            this.LEDEffectType = LEDEffectType;
            this.BaseColor = BaseColor;
            this.Element = Element;
            this.Timeout = Timeout;
            this.Speed = Speed;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            if (!(ArduinoCommand.robot.GetElementByUUID(Element) is LED element))
            {
                return;
            }
            switch (LEDEffectType)
            {
                case LEDEffectType.Frozen:
                    Frozen(element);
                    break;
                case LEDEffectType.Snakes:
                    Snakes(element);
                    break;
            }
        }

        private void Frozen(LED led)
        {
            led.SendColorToAll(BaseColor);
        }
        private void Snakes(LED led)
        {
            long started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            led.SendColorToAll(new Color(0, 0, 0));
            Thread.Sleep(50);
            int LoopNumber = 0;
            while (Running && (Timeout > 0 && DateTimeOffset.Now.ToUnixTimeMilliseconds() - started < Timeout))
            {
                LoopNumber++;
                if (LoopNumber > led.NombreLED)
                {
                    LoopNumber = 0;
                }
                led.SendPixelColor(LoopNumber, BaseColor, true);
                Thread.Sleep(30);
                //length = 5
                if (LoopNumber > 5)
                {
                    //length - 1 
                    led.SendPixelColor(LoopNumber - 6, Color.BLACK, true);
                }
                else
                {
                    led.SendPixelColor(led.NombreLED - 5 + LoopNumber, Color.BLACK, true);
                }
                Thread.Sleep(Speed);
            }

        }
    }

    public enum LEDEffectType
    {
        Frozen,
        Snakes,
    }
}
