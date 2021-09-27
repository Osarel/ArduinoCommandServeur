using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class OperationAction : AbstractAction
    {
        [JsonProperty]
        private readonly string Base;
        [JsonProperty]
        private readonly string Add;
        [JsonProperty]
        private readonly string To;
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly OperationType OperationType;

        [JsonConstructor]
        public OperationAction(OperationType OperationType, string Base, string Add, string To, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.OPERATION, false, ID, Position, Output)
        {
            this.Base = Base;
            this.Add = Add;
            this.To = To;
            this.OperationType = OperationType;
        }

        protected override void Launch(Sheet sheet, Liaison caller)
        {
            try
            {
                switch (OperationType)
                {
                    case OperationType.ADD:
                        sheet.SetVariable(To, sheet.ReadFloat(Base) + sheet.ReadFloat(Add));
                        break;
                    case OperationType.DIVISION:
                        sheet.SetVariable(To, sheet.ReadFloat(Base) / sheet.ReadFloat(Add));
                        break;
                    case OperationType.MULTIPLICATION:
                        sheet.SetVariable(To, sheet.ReadFloat(Base) * sheet.ReadFloat(Add));
                        break;
                    case OperationType.MODULO:
                        sheet.SetVariable(To, sheet.ReadFloat(Base) % sheet.ReadFloat(Add));
                        break;
                }
            } catch
            {

            }
        }


    }

    public enum OperationType
    {
        ADD,
        MULTIPLICATION,
        DIVISION,
        MODULO,
    }
}
