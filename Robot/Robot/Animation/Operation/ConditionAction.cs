using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConditionAction : ConditionableAction
    {

        [JsonConstructor]
        public ConditionAction(AnimatorConditionType ConditionType, string Condition, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONDITION, ConditionType, Condition, ID, Position, Output)
        {
        }
        protected override void Launch(Sheet sheet, Liaison caller)
        {

        }

        protected override void CallOutput(Sheet sheet)
        {
            //Si condition valide alors sortie 0 sinon sortie 1
            base.CallOutput(sheet, Output[ConditionCheck(sheet) ? 0 : 1]);
        }


    }

}
