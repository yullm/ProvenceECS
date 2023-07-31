using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ProvenceECS{

    public class CameraConverter : JsonConverter{
        public struct SerializedCamera{
            public string objectName;

            public SerializedCamera(Camera camera){
                this.objectName = camera.gameObject.name;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Camera));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedCamera sC = JsonConvert.DeserializeObject<SerializedCamera>(JToken.Load(reader).ToString());
            GameObject go = GameObject.Find(sC.objectName);
            if(go != null){
                Camera camera = go.GetComponent<Camera>();
                return camera;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedCamera sq = new SerializedCamera((Camera)value);
            JToken.FromObject(JsonConvert.SerializeObject(sq)).WriteTo(writer);
        }
    }

    public class QuaternionConverter : JsonConverter{
        public struct SerializedQuaternion{
            public float x;
            public float y;
            public float z;
            public float w;

            public SerializedQuaternion(float x, float y, float z, float w){
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Quaternion));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedQuaternion sq = JsonConvert.DeserializeObject<SerializedQuaternion>(JToken.Load(reader).ToString());
            return new Quaternion(sq.x,sq.y,sq.z,sq.w);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedQuaternion sq = new SerializedQuaternion(((Quaternion)value).x,((Quaternion)value).y,((Quaternion)value).z,((Quaternion)value).w);
            JToken.FromObject(JsonConvert.SerializeObject(sq)).WriteTo(writer);
        }

    }

    public class NullableQuaternionConverter : JsonConverter{
        public class NSerializedQuaternion{
            public float x;
            public float y;
            public float z;
            public float w;

            public NSerializedQuaternion(float x, float y, float z, float w){
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Quaternion?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            NSerializedQuaternion sq = JsonConvert.DeserializeObject<NSerializedQuaternion>(JToken.Load(reader).ToString());
            if(sq == null) return null;
            return new Quaternion(sq.x,sq.y,sq.z,sq.w);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            if(value != null){
                NSerializedQuaternion sq = new NSerializedQuaternion(((Quaternion)value).x,((Quaternion)value).y,((Quaternion)value).z,((Quaternion)value).w);
                JToken.FromObject(JsonConvert.SerializeObject(sq)).WriteTo(writer);
            }else writer.WriteNull();
        }

    }

    public class Vector4Converter : JsonConverter{

        public struct SerializedVector4{
            public float x;
            public float y;
            public float z;
            public float w;

            public SerializedVector4(float x, float y, float z, float w){
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Vector4));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            SerializedVector4 sv = JsonConvert.DeserializeObject<SerializedVector4>(JToken.Load(reader).ToString());
            return new Vector4(sv.x,sv.y,sv.z,sv.w);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            SerializedVector4 sv = new SerializedVector4(((Vector4)value).x,((Vector4)value).y,((Vector4)value).z,((Vector4)value).w);
            JToken.FromObject(JsonConvert.SerializeObject(sv)).WriteTo(writer);
        }
    }

    public class Vector3Converter : JsonConverter{

        protected struct SerializedVector3{
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

    public class NullableVector3Converter : JsonConverter{

        protected class NSerializedVector3{
            public float x;
            public float y;
            public float z;

            public NSerializedVector3(float x, float y, float z){
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(Vector3?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
            NSerializedVector3 sv = JsonConvert.DeserializeObject<NSerializedVector3>(JToken.Load(reader).ToString());
            if(sv == null) return null;
            return new Vector3(sv.x,sv.y,sv.z);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            if(value != null){
                NSerializedVector3 sv = new NSerializedVector3(((Vector3)value).x,((Vector3)value).y,((Vector3)value).z);
                JToken.FromObject(JsonConvert.SerializeObject(sv)).WriteTo(writer);
            }else writer.WriteNull();
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

        public struct SerializedGameObject{
            public string objectName;

            public SerializedGameObject(GameObject go){
                this.objectName = go.name;
            }
        }

        public override bool CanConvert(Type objectType){
            return (objectType == typeof(GameObject));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){            
            var token = JToken.Load(reader);
            if(token.Value<string>() == null) return null;
            SerializedGameObject sGO = JsonConvert.DeserializeObject<SerializedGameObject>(JToken.Load(reader).ToString());
            return GameObject.Find(sGO.objectName);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
            if(value != null){
                SerializedGameObject sGO = new SerializedGameObject((GameObject)value);
                JToken.FromObject(JsonConvert.SerializeObject(sGO)).WriteTo(writer);
            }
            else{
                writer.WriteNull();
            }
        }
    }

}