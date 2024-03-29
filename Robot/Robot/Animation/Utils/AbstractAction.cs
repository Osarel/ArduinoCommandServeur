﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;
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


        public Thread Routine { get; private set; }
        public Sheet sheet;

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
            this.sheet = sheet;
            sheet.log.LogDebug("Déclanchement de : {0}", Type);
            Running = true;
            sheet.currentAction.Add(ID);
            Routine = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = Async;
                Launch(caller);
                Stop(false);
                CallOutput();
            }
            );
            ArduinoCommand.eventG.FireActionStartedEvent(sheet, caller, this);
            Routine.Start();
            return Routine;
        }


        protected virtual void CallOutput()
        {
            foreach (Liaison value in Output)
            {
                sheet.StartAnimations(value);
            }
        }
        protected virtual void CallOutput(params Liaison[] args)
        {
            foreach (Liaison value in args)
            {
                sheet.StartAnimations(value);
            }
        }

        public virtual void Stop(bool force)
        {
            if (Routine != null && force)
            {
                Routine.Interrupt();
            }
            Running = false;
            sheet.currentAction.Remove(ID);
            ArduinoCommand.eventG.FireActionFinishEvent(sheet, this);
        }

        protected abstract void Launch(Liaison caller);

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
}
