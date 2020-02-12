using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class Helpers{

        public static List<Type> GetAllTypesFromBaseType(Type baseType, params Type[] additonalAssemblies){

            List<Type> types = new List<Type>();
            List<Type> baseTypes = new List<Type>(baseType.Assembly.GetTypes());

            foreach(Type type in additonalAssemblies){
                Type[] curTypes = type.Assembly.GetTypes();
                foreach(Type curType in curTypes) if(!baseTypes.Contains(curType)) baseTypes.Add(curType);
            }

            foreach(Type type in baseTypes){
                if(type.IsSubclassOf(baseType)) types.Add(type);
            }

            return types;
        }

        public static Type GetTypeFromStringInAssemblies(string typeString, Type baseType, params Type[] additonalAssemblies){

            foreach(Type type in  GetAllTypesFromBaseType(baseType, additonalAssemblies)){
                if(type.Name.Equals(typeString)) return type;
            }
            return null;
        }

        public static Type GetTypeFromStringInList(string typeString, List<Type> typeList){
            foreach(Type type in  typeList){
                if(type.Name.Equals(typeString)) return type;
            }
            return null;
        }

    }
    
}