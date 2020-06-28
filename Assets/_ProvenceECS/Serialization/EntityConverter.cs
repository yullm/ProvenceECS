using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ProvenceECS{

    public class EntityConverter : JsonConverter{
        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Entity) || objectType == typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            return new Entity(JToken.Load(reader).ToString());
        }

        public override void WriteJson(JsonWriter writer,  object value, JsonSerializer serializer){
            JToken.FromObject(value.ToString()).WriteTo(writer);
        }
    }

}