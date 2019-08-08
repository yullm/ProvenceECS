using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS{
    public class SystemWizard : EditorWindow
    {
        public SystemManager manager;
        public List<Type> types;

        void Awake(){
            types = new List<Type>();
            Type[] baseTypes = typeof(ProvenceSystem).Assembly.GetTypes();
            foreach(Type type in baseTypes){
                if(type.IsSubclassOf(typeof(ProvenceSystem))) types.Add(type);
            }
        }
        
        void OnGUI(){
            GUILayout.Label("Choose System");
            foreach(Type type in types){
                if(GUILayout.Button(type.Name)){
                    manager.AddSystemByType(type);
                    Close();
                }
            }
        }

    }
}