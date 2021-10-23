using Newtonsoft.Json;
using System.Collections.Generic;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class ExecutorAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Value;
        [JsonProperty]
        private readonly bool Executor = false;
        [JsonProperty]
        private readonly Dictionary<string, object> Variable;

        [JsonConstructor]
        public ExecutorAction(bool Async, string Value, Dictionary<string, object> Variable, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.EXECUTOR, Async, ID, Position, Output)
        {
            this.Value = Value;
            this.Variable = Variable;
        }

        protected override void Launch(Liaison caller)
        {
            if (!Executor)
            {
                Sheet launch = ArduinoCommand.robot.GetSheetByUUID(Value);
                if (launch == null)
                {
                    return;
                }
                launch.StartSheet(Variable);
            }
            else
            {
                switch (Value)
                {
                    case "STOP_MOVE":
                        ArduinoCommand.robot.StopMoveRobot();
                        break;
                    case "STOP_ROBOT":
                        ArduinoCommand.demande_arret = true;
                        break;
                    case "RESTART_ROBOT":
                        ArduinoCommand.demande_restart = true;
                        break;
                    case "STOP_COMPUTER":
                        ArduinoCommand.shutdown = true;
                        break;
                }
            }


        }
    }
}
