using Newtonsoft.Json;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class TalkAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Message;

        [JsonConstructor]
        public TalkAction(bool Async, string Message, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.TALK, Async, ID, Position, Output)
        {
            this.Message = Message;
        }

        protected override void Launch(Liaison caller)
        {
            Speaker.Say(Message);
        }
    }
}
