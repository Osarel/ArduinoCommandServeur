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
        public MoveAction(string From, string To, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.MOVE, false, ID, Position, Output)
        {
            this.From = From;
            this.To = To;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            sheet.SetVariable(To, sheet.ReadFloat(From));
        }


    }
}
