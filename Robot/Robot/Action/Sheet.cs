using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Robot.Action
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Sheet : IUpdatableElement
    {
        [JsonProperty]
        public readonly string ID;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public Dictionary<string, AbstractAction> action;
        [JsonProperty]
        public Dictionary<string, object> variable;
        [JsonProperty]
        public List<string> currentAction;
        [JsonProperty]
        public Liaison[] startupPoint;

        public ILogger log;
        [JsonConstructor]
        public Sheet(string ID, string Name, Dictionary<string, AbstractAction> action, Dictionary<string, object> variable, Liaison[] startupPoint)
        {
            this.ID = ID;
            this.Name = Name;
            this.action = action;
            this.variable = variable;
            this.startupPoint = startupPoint;
            currentAction = new List<string>();
            log = ArduinoCommand.loggerProvider.CreateLogger(Name);
        }

        public void StartAnimations(Liaison value)
        {
            if (action.TryGetValue(value.IDTo, out AbstractAction a))
            {
                a.Start(this, value);
            }
        }


        public bool StartSheet(Dictionary<string, object> variable)
        {
            ArduinoCommand.eventG.FireSheetStartedEvent(this, variable);
            if (currentAction.Count > 0)
            {
                return false;
            }
            if (variable != null)
            {
                this.variable = variable;
            }
            log.LogInformation("Démarrage de l'animation : " + Name);
            foreach (Liaison value in startupPoint)
            {
                Thread thread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    StartAnimations(value);
                });
                thread.Start();
            }
            return true;
        }

        public void ForceStopSheet()
        {
            foreach (string a in currentAction)
            {
                ForceStopAction(a);
            }
            ArduinoCommand.eventG.FireSheetFinishEvent(this);
        }

        public void ForceStopAction(string id)
        {
            if (action.TryGetValue(id, out AbstractAction a))
            {
                a.Stop(this, true);
            }
        }

        public void SetVariable(string to, object p)
        {
            variable[to] = p;
        }

        public float ReadFloat(string variable)
        {
            if (this.variable.TryGetValue(variable, out object value))
            {
                return (float)value;
            }
            if (ArduinoCommand.robot.Elements.TryGetValue(variable, out Element e))
            {
                return e.GetActualValue();
            }
            return 0;
        }

        public object GetVariable(string v)
        {
            return variable[v];
        }

        public bool ContainVariable(string v)
        {
            return variable.ContainsKey(v);
        }

        public bool Stop()
        {
            ForceStopSheet();
            return true;
        }

        public bool Save()
        {
            ArduinoCommand.robot.SaveAnimations();
            return true;
        }

        public bool AddToList()
        {
            ArduinoCommand.robot.Animations[ID] = this;
            return true;
        }

        public IUpdatableElement GetLastInstance()
        {
            return ArduinoCommand.robot.Animations[ID];
        }
    }
}
