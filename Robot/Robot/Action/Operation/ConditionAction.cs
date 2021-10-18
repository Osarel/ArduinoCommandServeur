using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConditionAction : AbstractAction
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnimatorConditionType ConditionType;
        [JsonProperty]
        public string Condition;

        [JsonConstructor]
        public ConditionAction(AnimatorConditionType ConditionType, string Condition, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONDITION, false, ID, Position, Output)
        {
            this.ConditionType = ConditionType;
            this.Condition = Condition;
        }
        protected override void Launch(Sheet sheet, Liaison caller)
        {

        }

        protected override void CallOutput(Sheet sheet)
        {
            //Si condition valide alors sortie 0 sinon sortie 1
            base.CallOutput(sheet, Output[ConditionCheck(sheet) ? 0 : 1]);
        }

        private bool ConditionCheck(Sheet sheet)
        {
            double value = 0;
            string[] conditionParse = Condition.Split(" ");
            if (conditionParse.Length != 3)
            {
                return true;
            }
            double at;
            try
            {
                at = Convert.ToDouble(conditionParse[2]);
            }
            catch
            {
                return true;
            }
            if (ConditionType == AnimatorConditionType.Capteur)
            {
                Element element = ArduinoCommand.robot.GetElementByUUID(conditionParse[0]);
                if (element == null)
                {
                    return true;
                }
                value = element.GetActualValue();
            }
            else if (ConditionType == AnimatorConditionType.Variable)
            {
                if (!sheet.ContainVariable(conditionParse[0]))
                {
                    return true;
                }
                //TODO 
                value = (double)sheet.GetVariable(conditionParse[0]);
            }
            return MadeCondition(value, at, conditionParse[1]);
        }

        private bool MadeCondition(double value, double at, string condition)
        {
            return condition switch
            {
                "==" => value == at,
                ">=" => value >= at,
                "<=" => value <= at,
                ">" => value > at,
                "<" => value < at,
                "!=" => value != at,
                _ => true,
            };
        }

    }

    public enum AnimatorConditionType
    {
        Capteur,
        Variable,
    }
}
