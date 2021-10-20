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
        public AnimatorEvenementType ConditionType;
        [JsonProperty]
        public readonly string Element;
        [JsonProperty]
        public readonly string Condition;
        [JsonProperty]
        public readonly int SecondArgument;

        public Dictionary<string, EventHandler<Event.Args.ElementActualValueChanged>> ObservatorList = new Dictionary<string, EventHandler<Event.Args.ElementActualValueChanged>>();
        [JsonConstructor]
        public EvenementAction(AnimatorEvenementType ConditionType, string Condition, string ID, Liaison.PointPosition Position, Liaison[] Output) : base(ActionType.CONDITION, false, ID, Position, Output)
        {
            this.ConditionType = ConditionType;
            this.Condition = Condition;
        }
        protected override void Launch(Sheet sheet, Liaison caller)
        {
            if (ConditionType == AnimatorEvenementType.Capteur)
            {
                CapteurEventDetection(sheet);
            } else if (ConditionType == AnimatorEvenementType.Serveur)
            {
                ServeurEventDetection(sheet);
            }
        }

        private void CapteurEventDetection(Sheet sheet)
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

        private void ServeurEventDetection(Sheet sheet)
        {
            //TODO
            //DETECTION DES EVENT SERVEUR ET ATTENTE DU BON
        }

        private void Element_ActualValueChangedHandler(object sender, Event.Args.ElementActualValueChanged e, Sheet sheet, string id)
        {
            if (Utils.MadeCondition(e.NewValue, SecondArgument, Condition))
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


    }

    public enum AnimatorEvenementType
    {
        Capteur,
        Serveur,
    }
}
