using Newtonsoft.Json;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConditionAction : ConditionableAction
    {

        [JsonConstructor]
        public ConditionAction(AnimatorConditionType ConditionType, string Condition, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONDITION, ConditionType, Condition, ID, Position, Output)
        {
        }
        protected override void Launch(Liaison caller)
        {

        }

        protected override void CallOutput()
        {
            //Si condition valide alors sortie 0 sinon sortie 1
            base.CallOutput(Output[ConditionCheck() ? 0 : 1]);
        }


    }

}
