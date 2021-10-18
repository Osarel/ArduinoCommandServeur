using Newtonsoft.Json;
using System.Threading;
using static Robot.Action.Liaison;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WaitAction : AbstractAction
    {
        [JsonProperty]
        private readonly int Temps;

        [JsonConstructor]
        public WaitAction(int Temps, string ID, PointPosition Position, Liaison[] Output) : base(ActionType.WAIT, false, ID, Position, Output)
        {
            this.Temps = Temps;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            Thread.Sleep(Temps);
        }
    }
}
