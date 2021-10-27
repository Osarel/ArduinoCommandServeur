using Newtonsoft.Json;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class TalkAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Message;

        [JsonConstructor]
        public TalkAction(bool Async, string Message, string ID, CubePositionAction Cube) : base(ActionType.TALK, Async, ID, Cube)
        {
            this.Message = Message;
        }

        protected override void Launch()
        {
            Speaker.Say(Message);
        }
    }
}
