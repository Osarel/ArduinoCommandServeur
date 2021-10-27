using Newtonsoft.Json;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WaitAction : AbstractAction
    {
        [JsonProperty]
        private readonly int Temps;

        [JsonConstructor]
        public WaitAction(int Temps, string ID, CubePositionAction Cube) : base(ActionType.WAIT, false, ID, Cube)
        {
            this.Temps = Temps;
        }

        protected override void Launch()
        {
            Thread.Sleep(Temps);
        }
    }
}
