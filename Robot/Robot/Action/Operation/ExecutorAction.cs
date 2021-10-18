using Newtonsoft.Json;
using System.Collections.Generic;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ExecutorAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Sheet;
        [JsonProperty]
        private readonly bool Executor = false;
        [JsonProperty]
        private readonly Dictionary<string, object> Variable;

        [JsonConstructor]
        public ExecutorAction(bool Async, string Sheet, Dictionary<string, object> Variable, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.EXECUTOR, Async, ID, Position, Output)
        {
            this.Sheet = Sheet;
            this.Variable = Variable;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            if (!Executor)
            {
                Sheet launch = ArduinoCommand.robot.GetSheetByUUID(Sheet);
                if (launch == null)
                {
                    return;
                }
                launch.StartSheet(Variable);
            } else
            {
                switch (Sheet)
                {
                    case "STOP_MOVE":
                        break;
                    case "STOP_ROBOT":
                        break;
                    case "RESTART_ROBOT":
                        break;
                }
            }


        }
    }
}
