using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ProvenceECS{

    public class Vector3Converter : JsonConverter{

        public struct SerializedVector3{
            public float x;
            public float y;
            public float z;

            public SerializedVector3(float x, float y, float z){
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Vector3));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedVector3 sv = JsonConvert.DeserializeObject<SerializedVector3>(JToken.Load(reader).ToString());
            return new Vector3(sv.x,sv.y,sv.z);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedVector3 sv = new SerializedVector3(((Vector3)value).x,((Vector3)value).y,((Vector3)value).z);
            JToken.FromObject(JsonConvert.SerializeObject(sv)).WriteTo(writer);
        }
    }

    public class Vector2Converter : JsonConverter{

        public struct SerializedVector2{
            public float x;
            public float y;

            public SerializedVector2(float x, float y){
                this.x = x;
                this.y = y;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Vector2));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedVector2 sv = JsonConvert.DeserializeObject<SerializedVector2>(JToken.Load(reader).ToString());
            return new Vector2(sv.x,sv.y);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedVector2 sv = new SerializedVector2(((Vector2)value).x,((Vector2)value).y);
            JToken.FromObject(JsonConvert.SerializeObject(sv)).WriteTo(writer);
        }
    }

    public class ColorConverter : JsonConverter{

        public struct SerializedColor{
            public float r;
            public float g;
            public float b;
            public float a;

            public SerializedColor(float r, float g, float b, float a){
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Color));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedColor sc = JsonConvert.DeserializeObject<SerializedColor>(JToken.Load(reader).ToString());
            return new Color(sc.r,sc.g,sc.b,sc.a);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedColor sv = new SerializedColor(((Color)value).r,((Color)value).g,((Color)value).b,((Color)value).a);
            JToken.FromObject(JsonConvert.SerializeObject(sv)).WriteTo(writer);
        }
    }

    public class GameObjectConverter : JsonConverter{
        public override bool CanConvert(Type objectType){
            return (objectType == typeof(GameObject));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            GameObject value = null;
            if(reader.Value != null) value = Resources.Load<GameObject>(reader.Value.ToString());
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            //writer.WriteValue(UnityEditor.AssetDatabase.GetAssetPath((GameObject)value));
        }
    }

}