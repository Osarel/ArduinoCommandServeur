using Newtonsoft.Json;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class MoveAction : AbstractAction
    {
        [JsonProperty]
        private readonly string From;
        [JsonProperty]
        private readonly string To;

        [JsonConstructor]
        public MoveAction(string From, string To, string ID, CubePositionAction Cube) : base(ActionType.MOVE, false, ID, Cube)
        {
            this.From = From;
            this.To = To;
        }

        protected override void Launch()
        {
            sheet.SetVariable(To, sheet.ReadFloat(From));
        }


    }
}
