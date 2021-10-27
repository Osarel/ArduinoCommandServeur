using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        public EvenementAction(AnimatorEvenementType ConditionType, string Condition, string Element, string ID, CubePositionAction Cube) : base(ActionType.CONDITION, false, ID, Cube)
        {
            this.ConditionType = ConditionType;
            this.Condition = Condition;
            this.Element = Element;
        }
        protected override void Launch()
        {
            if (ConditionType == AnimatorEvenementType.Capteur)
            {
                CapteurEventDetection();
            }
            else if (ConditionType == AnimatorEvenementType.Serveur)
            {
                ServeurEventDetection();
            }
        }

        private void CapteurEventDetection()
        {
            Element element = ArduinoCommand.robot.GetElementByUUID(Element);
            if (element == null)
            {
                base.Finish();
            }
            string observatorID = Guid.NewGuid().ToString();
            void handler(object sender, Event.Args.ElementActualValueChanged e) => Element_ActualValueChangedHandler(sender, e, observatorID);
            element.ActualValueChangedHandler += handler;
            ObservatorList.Add(observatorID, handler);
        }

        private void ServeurEventDetection()
        {
            //TODO
            //DETECTION DES EVENT SERVEUR ET ATTENTE DU BON
            EventInfo ev = typeof(Event.GlobalEvent).GetEvent(Element);
            MethodInfo handler = this.GetType()
            .GetMethod("EventHandlerGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
            var eh = Delegate.CreateDelegate(ev.EventHandlerType, this, handler);
            var minfo = ev.GetAddMethod();
            minfo.Invoke(ArduinoCommand.eventG, new object[] { eh });
            sheet.log.LogDebug("Ejout evenement : {0}", Element);
        }


        private void EventHandlerGeneric(object sender, EventArgs e)
        {
            EventInfo ev = typeof(Event.GlobalEvent).GetEvent(Element);
            MethodInfo handler = this.GetType()
            .GetMethod("GenericHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            var eh = Delegate.CreateDelegate(ev.EventHandlerType, this, handler);
            var removM = ev.GetRemoveMethod();
            removM.Invoke(ArduinoCommand.eventG, new object[] { eh });
            sheet.log.LogDebug("Evenement survenue {0} .. suite", Element);
            base.Finish();
        }

        private void Element_ActualValueChangedHandler(object sender, Event.Args.ElementActualValueChanged e, string id)
        {
            if (Utils.MadeCondition(e.NewValue, SecondArgument, Condition))
            {
                Next();
                Element element = (Element)sender;
                element.ActualValueChangedHandler -= ObservatorList[id];
                ObservatorList.Remove(id);
                base.Finish();

            }
        }

        protected override void Finish()
        {

        }


    }

    public enum AnimatorEvenementType
    {
        Capteur,
        Serveur,
    }
}
