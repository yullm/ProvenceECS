using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProvenceECS{
    [CustomEditor(typeof(ModifyComponentEvent))]
    public class ModifyComponentEditor : Editor{

        private SerializedObject obj;
        private ModifyComponentEvent targetEvent;
        private List<System.Type> componentTypes = new List<Type>();
        private bool chosenSet = false;

        private Editor embeddedInspector;
        

        public void OnEnable(){
            obj = new SerializedObject(target);
            targetEvent = (ModifyComponentEvent)target;
            componentTypes = Helpers.GetAllTypesFromBaseType(typeof(Component),typeof(Entity),typeof(UnityEngine.AI.NavMeshAgent));
            if(targetEvent.reflection) RecycleInspector(targetEvent.reflection);
            chosenSet = targetEvent.chosenComponent != null;
        }

        public override void OnInspectorGUI(){
            obj.Update();

            EditorGUILayout.PropertyField(obj.FindProperty("reflectionPool"));

            if(targetEvent.reflectionPool == null){
                EditorGUILayout.LabelField("Please Set a Reflection Pool Object");
                obj.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(obj.FindProperty("modifyType"));

            EditorGUILayout.BeginHorizontal();            
            if(targetEvent.reflection) EditorGUILayout.TextField(targetEvent.reflection.GetType().Name);

            if(GUILayout.Button("Change Type",GUILayout.Width(100))){
                TypeWizard typeWiz = (TypeWizard)EditorWindow.GetWindow(typeof(TypeWizard));
                typeWiz.Show(typeof(Component),ChangeType,typeof(Entity),typeof(UnityEngine.AI.NavMeshAgent)); 
            }
            EditorGUILayout.EndHorizontal();
            

            EditorGUILayout.PropertyField(obj.FindProperty("entityToModify"));


            if(targetEvent.modifyType == ModifyComponentEvent.ModifyType.EDIT){
                EditorGUILayout.PropertyField(obj.FindProperty("editAddIfMissing"));
                EditorGUILayout.PropertyField(obj.FindProperty("chosenComponent"));
                EditorGUILayout.PropertyField(obj.FindProperty("active"));
                if(!chosenSet && targetEvent.chosenComponent != null){
                    ChangeType(targetEvent.chosenComponent);
                    chosenSet = true;
                }
                if(chosenSet && targetEvent.chosenComponent == null){ 
                    chosenSet = false;
                }
            }

            if(targetEvent.modifyType == ModifyComponentEvent.ModifyType.ADD) 
                if(embeddedInspector) embeddedInspector.OnInspectorGUI();

            if(targetEvent.modifyType == ModifyComponentEvent.ModifyType.EDIT) 
                DisplayEditing();
            
            obj.ApplyModifiedProperties();
        }

        private void DisplayEditing(){
            if(targetEvent.reflection == null || targetEvent.editStates == null) return;
            SerializedObject sObj = new SerializedObject(targetEvent.reflection);
            var curProp = sObj.GetIterator();
            int index = 0;
            FieldInfo[] fields = targetEvent.reflection.GetType().GetFields();
            while(curProp.NextVisible(true)){
                EditorGUILayout.BeginHorizontal();
                if(index < fields.Length && fields[index].Name.Equals(curProp.name)){
                    targetEvent.editStates[index] = EditorGUILayout.Toggle(targetEvent.editStates[index],GUILayout.Width(15));
                    EditorGUILayout.LabelField(curProp.displayName,GUILayout.Width(100));
                    EditorGUILayout.PropertyField(curProp,GUIContent.none);
                    index++;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            curProp.Reset();
            sObj.ApplyModifiedProperties();
        }

        private void ChangeType(System.Type type){
            targetEvent.chosenComponent = null;
            if(targetEvent.reflection) DestroyImmediate(targetEvent.reflection);
            targetEvent.editStates = new bool[type.GetFields().Length];
            targetEvent.reflection = targetEvent.reflectionPool.AddComponent(type);
            RecycleInspector(targetEvent.reflection);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void ChangeType(Component chosen){
            ChangeType(chosen.GetType());
            targetEvent.chosenComponent = chosen;
            FieldInfo[] fields = chosen.GetType().GetFields();

            object[] fieldList = new object[fields.Length];
            for(int i = 0; i < fieldList.Length; i++){
                fields[i].SetValue(targetEvent.reflection,fields[i].GetValue(chosen));
            }
        }

        // a helper method that destroys the current inspector instance before creating a new one
        // use this in place of "Editor.CreateEditor"
        void RecycleInspector(UnityEngine.Object target){
            if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
            embeddedInspector = Editor.CreateEditor(target);
        }

        // clean up the inspector instance when the EditorWindow itself is destroyed
        void OnDisable(){
            if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
        }    
        
    }

}