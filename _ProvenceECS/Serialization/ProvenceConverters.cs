using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS{

    public class EntityConverter : JsonConverter{
        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Entity) || objectType == typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            var token = JToken.Load(reader);
            if(token.Value<string>() == null) return null;
            return new Entity(JToken.Load(reader).ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            if(value == null) writer.WriteNull();
            else JToken.FromObject(value.ToString()).WriteTo(writer);
        }
    }

    public class ProvenceAssetConverter<T> : JsonConverter where T : UnityEngine.Object{
        public override bool CanConvert(Type objectType){
            return objectType == typeof(ProvenceAsset<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            ProvenceAsset<T> asset = JsonConvert.DeserializeObject<ProvenceAsset<T>>(JToken.Load(reader).ToString());
            if(!asset.resourcePath.Equals("")) asset.asset = Resources.Load<T>(asset.resourcePath);
            return asset;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            JToken.FromObject(JsonConvert.SerializeObject(value)).WriteTo(writer);
        }
    }

}