using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProvenceECS{

    [CustomEditor(typeof(SystemManager))]
    public class SystemManagerEditor : Editor
    {
        private SerializedObject obj;

        public void OnEnable(){
            obj = new SerializedObject(target);
        }
        
        public override void OnInspectorGUI(){
            obj.Update();
            EditorGUILayout.PropertyField(obj.FindProperty("world"),true);
            List<ProvenceSystem> list = ((SystemManager)target).systems;
            //EditorGUILayout.PropertyField(obj.FindProperty("systems"),true);

            for(int i = 0; i < list.Count; i++){
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(obj.FindProperty("systems").GetArrayElementAtIndex(i),new GUIContent(""));
                if(GUILayout.Button("Remove")){
                    list.Remove(list[i]);
                    obj.ApplyModifiedProperties();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    return;
                }
                EditorGUILayout.EndHorizontal();

            }

            if(GUILayout.Button("Add System")){
                List<System.Type> existingTypes = new List<System.Type>();
                foreach(ProvenceSystem system in ((SystemManager)target).systems){
                    existingTypes.Add(system.GetType());
                }
                TypeWizard typeWiz = (TypeWizard)EditorWindow.GetWindow(typeof(TypeWizard));
                typeWiz.Show(typeof(ProvenceSystem), existingTypes, AddSystem);
            }
            obj.ApplyModifiedProperties();
        }

        private void AddSystem(System.Type type){
            bool has = false;
            foreach(ProvenceSystem system in ((SystemManager)target).systems){
                if(type == system.GetType()){
                    has = true;
                    break;
                }
            }
            if(!has) ((SystemManager)target).AddSystemByType(type);
        }
    }
}