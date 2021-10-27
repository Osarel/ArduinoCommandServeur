using Newtonsoft.Json;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class DivergenceAction : AbstractAction
    {
        [JsonConstructor]
        public DivergenceAction(string ID, CubePositionAction Cube) : base(ActionType.DIVERGENCE, false, ID, Cube)
        {
        }

        protected override void Launch()
        {

        }

        protected override void Next()
        {
            Top();
            Bottom();
        }


    }
}
