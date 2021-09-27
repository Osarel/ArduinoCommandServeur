using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;
using System;
using static Robot.Action.Liaison;

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
        public int State = 0;
        [JsonProperty]
        public Liaison[] Output;
        [JsonProperty]
        public PointPosition Position;


        public Thread thread { get; private set; }

        [JsonConstructor]
        public AbstractAction(ActionType Type, bool Async, string ID, PointPosition Position, Liaison[] Output)
        {
            this.Type = Type;
            this.Async = Async;
            this.ID = ID;
            this.Position = Position;
            this.Output = Output;
        }

        public virtual Thread Start(Sheet sheet, Liaison caller)
        {
            if (Running)
            {
                return null; 
            }
            if (ArduinoCommand.robot.Option.debug)
            {
                Console.WriteLine("Déclanchement de : " + Type);
            }
            Running = true;
            sheet.currentAction.Add(ID);
            thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = Async;
                Launch(sheet, caller);
                Stop(sheet);
                CallOutput(sheet);
            }
            );
            thread.Start();
            return thread;
        }


        protected virtual void CallOutput(Sheet sheet)
        {
            foreach (Liaison value in Output)
            {
                sheet.StartAnimations(value);
            }
        }
        protected virtual void CallOutput(Sheet sheet, params Liaison[] args)
        {
            foreach (Liaison value in args)
            {
                sheet.StartAnimations(value);
            }
        }

        public virtual void Stop(Sheet sheet)
        {
            Running = false;
            sheet.currentAction.Remove(ID);
            if (ArduinoCommand.robot.Option.debug)
            {
                Console.WriteLine("Arret de : " + Type);
            }
        }

        public virtual void ForceStop()
        {
            if (thread != null)
            {
                thread.Abort();
            }
            Running = false;
            if (ArduinoCommand.robot.Option.debug)
            {
                Console.WriteLine("Arret de : " + Type);
            }
        }


        protected abstract void Launch(Sheet sheet, Liaison caller);

    }

    public enum ActionType{
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
}
