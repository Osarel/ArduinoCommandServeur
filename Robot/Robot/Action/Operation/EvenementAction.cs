using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    class EvenementAction : AbstractAction
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnimatorConditionType ConditionType;
        [JsonProperty]
        public readonly string Element;
        [JsonProperty]
        public readonly string Condition;
        [JsonProperty]
        public readonly int SecondArgument;

        public Dictionary<string, EventHandler<Event.Args.ElementActualValueChanged>> ObservatorList = new Dictionary<string, EventHandler<Event.Args.ElementActualValueChanged>>();
        [JsonConstructor]
        public EvenementAction(AnimatorConditionType ConditionType, string Condition, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONDITION, false, ID, Position, Output)
        {
            this.ConditionType = ConditionType;
            this.Condition = Condition;
        }
        protected override void Launch(Sheet sheet, Liaison caller)
        {
            if (ConditionType == AnimatorConditionType.Capteur)
            {
                Element element = ArduinoCommand.robot.GetElementByUUID(Element);
                if (element == null)
                {
                    CallOutput(sheet); //E
                }
                string observatorID = Guid.NewGuid().ToString();
                void handler(object sender, Event.Args.ElementActualValueChanged e) => Element_ActualValueChangedHandler(sender, e, sheet, observatorID);
                element.ActualValueChangedHandler += handler;
                ObservatorList.Add(observatorID, handler);
            }
        }

        private void Element_ActualValueChangedHandler(object sender, Event.Args.ElementActualValueChanged e, Sheet sheet, string id)
        {
            if (MadeCondition(e.NewValue, SecondArgument, Condition))
            {
                CallOutput(sheet);
                Element element = (Element)sender;
                element.ActualValueChangedHandler -= ObservatorList[id];
                ObservatorList.Remove(id);
                base.Stop(sheet, false);

            }
        }

        public override Thread Start(Sheet sheet, Liaison caller)
        {
            if (Running)
            {
                return null;
            }
            if (ArduinoCommand.robot.Options.debug)
            {
                sheet.log.LogDebug("Déclanchement de : " + Type);
            }
            Running = true;
            sheet.currentAction.Add(ID);
            Thread thread = new Thread(() =>
            {

                Launch(sheet, caller);
            }
            );
            thread.Start();
            thread.Join();
            return thread;
        }


        private bool MadeCondition(double value, double at, string condition)
        {
            switch (condition)
            {
                case "==":
                    return value == at;
                case ">=":
                    return value >= at;
                case "<=":
                    return value <= at;
                case ">":
                    return value > at;
                case "<":
                    return value < at;
                case "!=":
                    return value != at;
                default:
                    return false;
            }
        }

    }

    public enum AnimatorEvenementType
    {
        Capteur,
    }
}
