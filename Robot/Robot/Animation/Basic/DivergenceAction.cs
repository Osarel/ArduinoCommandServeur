using Newtonsoft.Json;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class DivergenceAction : AbstractAction
    {
        [JsonConstructor]
        public DivergenceAction(string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.DIVERGENCE, false, ID, Position, Output)
        {
        }

        protected override void Launch(Liaison caller)
        {

        }

        protected override void CallOutput()
        {
            foreach (Liaison value in Output)
            {
                Thread thread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    sheet.StartAnimations(value);
                });
                thread.Start();
            }
        }


    }
}
