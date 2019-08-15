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
            EditorGUILayout.PropertyField(obj.FindProperty("systems"),true);
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