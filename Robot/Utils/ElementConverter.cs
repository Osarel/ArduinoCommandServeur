using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Robot
{
    public class ElementSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Element).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ElementConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ElementSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Element));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return Enum.Parse(typeof(ElementType), jo["Type"].Value<string>()) switch
            {
                ElementType.SERVOMOTOR => JsonConvert.DeserializeObject<ServoMotor>(jo.ToString(), SpecifiedSubclassConversion),
                ElementType.LED => JsonConvert.DeserializeObject<LED>(jo.ToString(), SpecifiedSubclassConversion),
                ElementType.PIN => JsonConvert.DeserializeObject<PIN>(jo.ToString(), SpecifiedSubclassConversion),
                _ => throw new Exception(),
            };
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
