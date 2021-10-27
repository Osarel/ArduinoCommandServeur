using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConvergenceAction : AbstractAction
    {

        [JsonProperty]
        public readonly int Input;

        public int finish = 0;
        [JsonConstructor]
        public ConvergenceAction(string ID, int Input, CubePositionAction Cube) : base(ActionType.CONVERGENCE, false, ID, Cube)
        {
            this.Input = Input;
        }


        protected override void Launch()
        {
            finish++;
            if (finish >= Input)
            {
                finish = 0;
                base.Finish();
            }
        }

        protected override void Finish()
        {
            Stop(false);
        }

    }
}
