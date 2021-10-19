using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ConvergenceAction : AbstractAction
    {

        [JsonProperty]
        public readonly int Input;
        public static IDictionary<string, int> Finish = new Dictionary<string, int>();
        [JsonConstructor]
        public ConvergenceAction(string ID, int Input, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONVERGENCE, false, ID, Position, Output)
        {
            Finish[ID] = 0;
            this.Input = Input;
        }

        public override Thread Start(Sheet sheet, Liaison caller)
        {
            Launch(sheet, caller);

            return Routine;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            Finish[ID] = Finish[ID] + 1;
            if (Finish[ID] >= Input)
            {
                if (ArduinoCommand.robot.Options.debug)
                {
                    Console.WriteLine("Passage de la convergence");
                }
                Finish[ID] = 0;
                base.CallOutput(sheet);
            }
            else
            {
                CallOutput(sheet);
            }
        }

        protected override void CallOutput(Sheet sheet)
        {
        }


    }
}
