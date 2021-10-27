using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(ActionConverter))]
    public abstract class AbstractAction
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type;
        [JsonProperty]
        public bool Async = false;
        [JsonProperty]
        public bool Running = false;
        [JsonProperty]
        public string ID { get; private set; }

        [JsonProperty]
        public CubePositionAction Cube;

        public string idCaller;
        public Thread Routine { get; private set; }
        public Sheet sheet;

        [JsonConstructor]
        public AbstractAction(ActionType Type, bool Async, string ID, CubePositionAction Cube)
        {
            this.Type = Type;
            this.Async = Async;
            this.ID = ID;
            this.Cube = Cube;
        }

        public virtual Thread Start(Sheet sheet, string caller)
        {
            //Si deja en cours on stop ici
            if (Running)
            {
                return null;
            }

            //Définition variable de fonctionnement 
            this.sheet = sheet;
            this.idCaller = caller;
            sheet.log.LogDebug("Déclanchement de : {0}", Type);
            Running = true;
            sheet.currentAction.Add(this);

            //Déclanchement de la routine
            Routine = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = Async;
                Launch();
                Finish();
            }
            );
            ArduinoCommand.eventG.FireActionStartedEvent(sheet, this);
            Routine.Start();
            return Routine;
        }

        protected virtual void Finish()
        {
            Stop(false);
            Next();
        }

        public static bool SecureStart(AbstractAction action, Sheet sheet, string IDCaller, string IDBase)
        {
            if (action == null)
            {
                return false;
            }
            if (action.ID == IDBase)
            {
                return false;
            }

            action.Start(sheet, IDCaller);

            return true;
        }

        public static bool SecureStart(AbstractAction action, Sheet sheet, string IDCaller)
        {
           return SecureStart(action, sheet, IDCaller, "");
        }
        protected virtual void Next()
        {
            SecureStart(Cube.Right, sheet,  ID, idCaller);
        }

        protected virtual void Top()
        {
            SecureStart(Cube.Right, sheet, idCaller);
        }

        protected virtual void Bottom()
        {
            SecureStart(Cube.Right, sheet, idCaller);
        }

        public virtual void Stop(bool force)
        {
            if (Routine != null && force)
            {
                Routine.Interrupt();
            }
            Running = false;
            sheet.currentAction.Remove(this);
            ArduinoCommand.eventG.FireActionFinishEvent(sheet, this);
        }

        protected abstract void Launch();

    }

    public enum ActionType
    {
        WAIT,
        SERVO,
        DIGITAL,
        ANALOG,
        TALK,
        CONDITION,
        EVENEMENT,
        CONVERGENCE,
        DIVERGENCE,
        MOVE,
        OPERATION,
        LED,
        EXECUTOR
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CubePositionAction
    {
        [JsonProperty]
        public AbstractAction Top;
        [JsonProperty]
        public AbstractAction Right;
        [JsonProperty]
        public AbstractAction Bottom;
    }
}

