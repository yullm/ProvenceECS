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
            EditorGUILayout.PropertyField(obj.FindProperty("systems"),true);
            for(int i = 0; i < ((SystemManager)target).systems.Count; i++){
                GUILayout.Label(((SystemManager)target).systems[i].GetType().ToString());
            }
            if(GUILayout.Button("Add System")){
                SystemWizard wiz = (SystemWizard)EditorWindow.GetWindow(typeof(SystemWizard));
                wiz.manager = target as SystemManager;
                wiz.Show();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            obj.ApplyModifiedProperties();
        }
    }
}