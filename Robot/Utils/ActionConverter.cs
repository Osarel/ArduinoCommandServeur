using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Robot.Action;
using System;

namespace Robot
{
    public class ActionSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(AbstractAction).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ActionConverter : JsonConverter
    {
        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ActionSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(AbstractAction));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (Enum.Parse(typeof(ActionType), jo["Type"].Value<string>()))
            {
                case ActionType.ANALOG:
                    return JsonConvert.DeserializeObject<AnaloguePINAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.CONDITION:
                    return JsonConvert.DeserializeObject<ConditionAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.CONVERGENCE:
                    return JsonConvert.DeserializeObject<ConvergenceAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.EVENEMENT:
                    return JsonConvert.DeserializeObject<EvenementAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.DIGITAL:
                    return JsonConvert.DeserializeObject<DigitalPINAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.DIVERGENCE:
                    return JsonConvert.DeserializeObject<DivergenceAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.EXECUTOR:
                    return JsonConvert.DeserializeObject<ExecutorAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.LED:
                    return JsonConvert.DeserializeObject<LEDEffectAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.MOVE:
                    return JsonConvert.DeserializeObject<MoveAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.OPERATION:
                    return JsonConvert.DeserializeObject<OperationAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.SERVO:
                    return JsonConvert.DeserializeObject<ServoAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.TALK:
                    return JsonConvert.DeserializeObject<TalkAction>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.WAIT:
                    return JsonConvert.DeserializeObject<WaitAction>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
