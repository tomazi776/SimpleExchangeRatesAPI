using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class ApiCurrencyNameConverter : JsonConverter
    {

        public IDictionary<string, string> PropertyMappings = new Dictionary<string, string>() 
        {
            { "0:0:0:0:0", "CurrencyInjectedFromCtor"},
            { "0:1:0:0:0", "CurrencyInjectedFromCtor"},
        };

        // TODO add iterator for propMapping keys representing consecutive currencies
        public ApiCurrencyNameConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsClass;
        }

        public void AddPropToConvert(string prop, string newProp)
        {
            PropertyMappings.Add(prop, newProp);
        }

        //Use reflection to act on class properties
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object instance = Activator.CreateInstance(objectType);
            var props = objectType.GetTypeInfo().DeclaredProperties.ToList();

            JObject jo = JObject.Load(reader);
            foreach (JProperty jp in jo.Properties())
            {
                if (!PropertyMappings.TryGetValue(jp.Name, out var name))
                    name = jp.Name;

                PropertyInfo prop = props.FirstOrDefault(pi =>
                    pi.CanWrite && pi.GetCustomAttribute<JsonPropertyAttribute>().PropertyName == name);

                prop?.SetValue(instance, jp.Value.ToObject(prop.PropertyType, serializer));
            }
            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
