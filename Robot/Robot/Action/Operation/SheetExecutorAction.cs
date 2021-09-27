using Newtonsoft.Json;
using System.Collections.Generic;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class SheetExecutorAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Sheet;
        [JsonProperty]
        private readonly Dictionary<string, object> Variable;

        [JsonConstructor]
        public SheetExecutorAction(bool Async, string Sheet, Dictionary<string, object> Variable, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.EXECUTOR, Async, ID, Position, Output)
        {
            this.Sheet = Sheet;
            this.Variable = Variable;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            Sheet launch = ArduinoCommand.robot.GetSheetByUUID(Sheet);
            if (launch == null)
            {
                return;
            }
            launch.StartSheet(Variable);
        }
    }
}
