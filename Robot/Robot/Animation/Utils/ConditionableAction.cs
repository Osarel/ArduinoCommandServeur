using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Robot.Action
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ConditionableAction : AbstractAction
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnimatorConditionType ConditionType;
        [JsonProperty]
        public string Condition;

        [JsonConstructor]
        public ConditionableAction(ActionType type, AnimatorConditionType ConditionType, string Condition, string ID, CubePositionAction Cube) : base(type, false, ID, Cube)
        {
            this.ConditionType = ConditionType;
            this.Condition = Condition;
        }
        protected override void Launch()
        {

        }

        public bool ConditionCheck()
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
            return Utils.MadeCondition(value, at, conditionParse[1]);
        }



    }
    public enum AnimatorConditionType
    {
        Capteur,
        Variable,
    }


}
