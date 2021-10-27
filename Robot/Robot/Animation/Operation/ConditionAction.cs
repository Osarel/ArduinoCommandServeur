using Newtonsoft.Json;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConditionAction : ConditionableAction
    {

        [JsonConstructor]
        public ConditionAction(AnimatorConditionType ConditionType, string Condition, string ID, CubePositionAction Cube) : base(ActionType.CONDITION, ConditionType, Condition, ID, Cube)
        {
        }
        protected override void Launch()
        {

        }

        protected override void Next()
        {
            //Si condition valide alors sortie Top sinon sortie Bottom
            if (ConditionCheck())
            {
                 Top();
            } else
            {
                 Bottom();
            }
        }


    }

}
