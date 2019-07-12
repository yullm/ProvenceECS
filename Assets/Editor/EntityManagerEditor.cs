using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EntityManager))]
public class EntityManagerEditor : Editor
{

    private SerializedObject obj;

    public void OnEnable(){
        obj = new SerializedObject(target);
    }

    private void DrawEntities(){
        
        SerializedProperty entityKeys = obj.FindProperty("entities.keys");
        SerializedProperty entityValues = obj.FindProperty("entities.values");

        GUILayout.Label("Available Entities: " + obj.FindProperty("availableEntities").arraySize, EditorStyles.boldLabel);  

        GUILayout.Label("Live Entities: ", EditorStyles.boldLabel);  
        EditorGUI.indentLevel += 1;
        for(int i = 0; i < entityKeys.arraySize; i++){
            
            int entityId = entityKeys.GetArrayElementAtIndex(i).FindPropertyRelative("id").intValue;
            GameObject go = entityValues.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            if(go && go.activeSelf){
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entityKeys.GetArrayElementAtIndex(i).FindPropertyRelative("id").intValue.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(30));
                EditorGUILayout.PropertyField(entityValues.GetArrayElementAtIndex(i),GUIContent.none);
                
                if(GUILayout.Button("Select")){
                    Selection.objects = new GameObject[]{go};
                }
                if(GUILayout.Button("Remove")){
                    ((EntityManager)target).RemoveEntity(entityId);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    Selection.selectionChanged();
                }
                
                EditorGUILayout.EndHorizontal();
           }
        }

        EditorGUI.indentLevel -= 1;
        
    }

    public override void OnInspectorGUI(){
        obj.Update();

        GUILayout.Label("World ID: " + ((EntityManager)target).world.id, EditorStyles.boldLabel); 

        GUILayout.Label("Methods: ", EditorStyles.boldLabel);  
        if(GUILayout.Button("Create Entity")){
            ((EntityManager)target).CreateEntity();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.selectionChanged();
        }
        if(GUILayout.Button("Clear Entities")){
            ((EntityManager)target).ClearEntities();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.selectionChanged();
        }        
        DrawEntities();
        obj.ApplyModifiedProperties();
    }
}
