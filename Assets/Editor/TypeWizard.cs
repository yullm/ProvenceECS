using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS{

    public delegate void TypeWizardDelegate (Type e);

    public class TypeWizard : EditorWindow{
        public List<Type> typeList = new List<Type>();
        private List<Type> displayList = new List<Type>();
        public string searchString = "";
        private string previousSearchValue = "someValue";
        private Vector2 scrollPos = Vector2.zero;
        private TypeWizardDelegate callback;

        public void Show(Type baseType, TypeWizardDelegate del, params Type[] additonalAssemblies){
            callback = del;
            typeList = Helpers.GetAllTypesFromBaseType(baseType, additonalAssemblies);
        }

        public void Show(Type baseType, List<Type> removeExistingList, TypeWizardDelegate del, params Type[] additonalAssemblies){
            callback = del;
            List<Type> baseTypes = new List<Type>(baseType.Assembly.GetTypes());

            foreach(Type type in additonalAssemblies){
                Type[] curTypes = type.Assembly.GetTypes();
                foreach(Type curType in curTypes) if(!baseTypes.Contains(curType)) baseTypes.Add(curType);
            }

            foreach(Type type in baseTypes){
                if(type.IsSubclassOf(baseType) && !removeExistingList.Contains(type)) typeList.Add(type);
            }
        }

        public void FilterDisplayList(){
            displayList.Clear();
            if(searchString.Equals("")){
                displayList.AddRange(typeList);
                return;
            }
            foreach(Type curType in typeList){
                string typename = curType.Name.ToLower();
                if(typename.Contains(searchString.ToLower())) displayList.Add(curType);
            }
        }

        void OnGUI(){
            searchString = EditorGUILayout.TextField(searchString);
            if(!searchString.Equals(previousSearchValue)) FilterDisplayList();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach(Type type in displayList){
                if(GUILayout.Button(type.Name)){
                    callback.Invoke(type);
                    Close();
                }
            }
            previousSearchValue = searchString;
            EditorGUILayout.EndScrollView();
        }

        

    }

}