using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceContractResolver : DefaultContractResolver{

    

        protected override JsonContract CreateContract(Type objectType){
            JsonContract contract = base.CreateContract(objectType);

            //if(objectType == typeof(sbyte)) contract.Converter = new SByteConverter(); 
            if(objectType == typeof(Entity)) contract.Converter = new EntityConverter();            
            if(objectType == typeof(Quaternion)) contract.Converter = new QuaternionConverter();
            if(objectType == typeof(Quaternion?)) contract.Converter = new NullableQuaternionConverter();
            if(objectType == typeof(Vector4)) contract.Converter = new Vector4Converter();
            if(objectType == typeof(Vector3)) contract.Converter = new Vector3Converter();
            if(objectType == typeof(Vector3?)) contract.Converter = new NullableVector3Converter();
            if(objectType == typeof(Vector2)) contract.Converter = new Vector2Converter();
            if(objectType == typeof(Color)) contract.Converter = new ColorConverter();
            if(objectType == typeof(Camera)) contract.Converter = new CameraConverter();
            if(objectType == typeof(GameObject)) contract.Converter = new GameObjectConverter();
            if(objectType.IsGenericType){
                if(objectType.GetGenericTypeDefinition() == typeof(ProvenceAsset<>).GetGenericTypeDefinition()){
                    Type genericType = objectType.GetGenericArguments()[0];
                    var converter = Activator.CreateInstance(typeof(ProvenceAssetConverter<>).MakeGenericType(genericType));
                    contract.Converter = (JsonConverter)converter;
                }
            }

            return contract;
        }
    }

}