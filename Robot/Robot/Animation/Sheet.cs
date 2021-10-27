using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        public Dictionary<string, object> variable;
        [JsonProperty]
        public AbstractAction startupPoint;

        public List<AbstractAction> currentAction = new List<AbstractAction>();
        public ILogger log;
        [JsonConstructor]
        public Sheet(string ID, string Name, Dictionary<string, object> variable, AbstractAction startupPoint)
        {
            this.ID = ID;
            this.Name = Name;
            this.variable = variable;
            this.startupPoint = startupPoint;
            log = ArduinoCommand.loggerProvider.CreateLogger(Name);
        }

        public bool StartSheet(Dictionary<string, object> variable)
        {
            ArduinoCommand.eventG.FireSheetStartedEvent(this, variable);

            if (variable != null)
            {
                this.variable = variable;
            }
            log.LogInformation("Démarrage de l'animation : " + Name);
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                AbstractAction.SecureStart(startupPoint, this, "main");
            });
            thread.Start();
            return true;
        }

        public void ForceStopSheet()
        {
            foreach (AbstractAction a in currentAction)
            {
                a.Stop(true);
            }
            ArduinoCommand.eventG.FireSheetFinishEvent(this);
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
