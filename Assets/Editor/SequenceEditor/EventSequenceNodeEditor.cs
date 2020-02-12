using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProvenceECS{

    public class EventSequenceNodeEditor : EditorWindow{
        private EventSequenceNode node;
        private Vector2 scrollPos = Vector2.zero;
        private Editor embeddedInspector;
        private string descriptor;

        public static void Show(EventSequenceNode node, string descriptor){
            EventSequenceNodeEditor window = (EventSequenceNodeEditor)GetWindow<EventSequenceNodeEditor>();
            window.node = node;
            window.descriptor = descriptor;
            if(window.node.sequencedEvent.provenceEvent != null) window.RecycleInspector(window.node.sequencedEvent.provenceEvent);
            AssemblyReloadEvents.beforeAssemblyReload += () => window.Close();
        }

        private void OnGUI(){
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.ExpandWidth(true));
            
            DrawSequenceParameters();
            DrawConditionalEvent();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSequenceParameters(){
            if(node.sequencedEvent.provenceEvent == null){
                if(GUI.Button(new Rect(10, (position.height/2) - 15, position.width - 20, 30),"Select Event Type")){
                    TypeWizard typeWiz = (TypeWizard)EditorWindow.GetWindow(typeof(TypeWizard));
                    typeWiz.Show(typeof(ProvenceEventArgs),SetEventType,typeof(Entity));
                }
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(node.sequencedEvent.provenceEvent.GetType().Name);
            if(GUILayout.Button("Change Event Type",GUILayout.Width(150))){
                TypeWizard typeWiz = (TypeWizard)EditorWindow.GetWindow(typeof(TypeWizard));
                typeWiz.Show(typeof(ProvenceEventArgs),SetEventType,typeof(Entity));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Allow Failiure?",GUILayout.Width(100));
            bool currentVal = node.allowFailiure;
            node.allowFailiure = EditorGUILayout.Toggle(node.allowFailiure,GUILayout.Width(15));
            if(currentVal != node.allowFailiure) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorGUILayout.EndHorizontal();
        }

        private void DrawConditionalEvent(){
            if(node.sequencedEvent != null && node.sequencedEvent.provenceEvent != null){
                SerializedObject serializedEvent = new SerializedObject(node.sequencedEvent.provenceEvent);

                EditorGUILayout.LabelField("EVENT:");
                if(embeddedInspector) embeddedInspector.OnInspectorGUI();

                DrawSwitchList(serializedEvent);
                DrawComponentList(serializedEvent);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                serializedEvent.ApplyModifiedProperties();
            }
        }

        private void DrawSwitchList(SerializedObject serializedEvent){
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("SWITCH CONDITIONS:");

                for(int i = 0; i < node.sequencedEvent.switchList.Count; i++){
                    EditorGUILayout.BeginHorizontal();
                    node.sequencedEvent.switchList[i].conditionName = EditorGUILayout.TextField(node.sequencedEvent.switchList[i].conditionName,GUILayout.ExpandWidth(true));
                    node.sequencedEvent.switchList[i].conditionState = EditorGUILayout.Toggle(node.sequencedEvent.switchList[i].conditionState,GUILayout.Width(15));
                    if(GUILayout.Button("Remove",GUILayout.Width(100))){
                        node.sequencedEvent.switchList.Remove(node.sequencedEvent.switchList[i]);
                        serializedEvent.ApplyModifiedProperties();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if(GUILayout.Button("ADD SWITCH CONDITION")){
                    node.sequencedEvent.switchList.Add(new StringConditional());
                    serializedEvent.ApplyModifiedProperties();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                }
        }

        private void DrawComponentList(SerializedObject serializedEvent){
            EditorGUILayout.Space();
                EditorGUILayout.LabelField("COMPONENT CONDITIONS FOR " + descriptor + " ENTITY:");

                for(int i = 0; i < node.sequencedEvent.componentList.Count; i++){
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.TextField(node.sequencedEvent.componentList[i].conditionName, GUILayout.ExpandWidth(true));
                    node.sequencedEvent.componentList[i].conditionState = EditorGUILayout.Toggle(node.sequencedEvent.componentList[i].conditionState ,GUILayout.Width(15));
                    if(GUILayout.Button("Remove",GUILayout.Width(100))){
                        node.sequencedEvent.componentList.Remove(node.sequencedEvent.componentList[i]);
                        serializedEvent.ApplyModifiedProperties();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if(GUILayout.Button("ADD COMPONENT CONDITION")){
                    TypeWizard typeWiz = (TypeWizard)EditorWindow.GetWindow(typeof(TypeWizard));
                    typeWiz.Show(typeof(Component),AddComponentCondition,typeof(Entity));
                }
        }

        private void SetEventType(Type type){
            node.sequencedEvent.provenceEvent = ScriptableObject.CreateInstance(type) as ProvenceEventArgs;
            RecycleInspector(node.sequencedEvent.provenceEvent);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void AddComponentCondition(Type type){
            node.sequencedEvent.componentList.Add(new StringConditional(type.Name));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        void RecycleInspector(UnityEngine.Object target){
            if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
            embeddedInspector = Editor.CreateEditor(target);
        }

        void OnDisable(){
            if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
            AssemblyReloadEvents.beforeAssemblyReload -= () => Close();
        }

        /* void OnLostFocus(){
            Close();
        } */

    }

}