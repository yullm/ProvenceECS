using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceContractResolver : DefaultContractResolver{

        protected override JsonContract CreateContract(Type objectType){
            JsonContract contract = base.CreateContract(objectType);

            if(objectType == typeof(Entity)) contract.Converter = new EntityConverter();
            if(objectType == typeof(Vector4)) contract.Converter = new Vector4Converter();
            if(objectType == typeof(Vector3)) contract.Converter = new Vector3Converter();
            if(objectType == typeof(Vector2)) contract.Converter = new Vector2Converter();
            if(objectType == typeof(Color)) contract.Converter = new ColorConverter();
            //if(objectType == typeof(GameObject)) contract.Converter = new GameObjectConverter();

            return contract;
        }
    }

}