using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class ProvenceECS : EditorWindow
{
    private const string dir = "ProvenceECS/";
    private WorldManager worldManager;
    private SerializedObject so;
    private GUISkin defaultSkin;
    //DISPLAY VARIABLES
    private bool creatingWorld = false;
    string newWorldName = "New World";
    World selectedWorld;
    SerializedObject serializedEntityManager;
    int worldToolbarIndex = 0;
    string[] worldToolbarTitles = {"Entities","Systems","Events"};
    //END DISPLAY VARIABLES

    [MenuItem(dir + "Open")]
    public static void ShowWindow(){
        EditorWindow.GetWindow(typeof(ProvenceECS));  
    }

    public void Awake(){
        Debug.Log("Awakened");
        EditorSceneManager.sceneOpened += Initialize;
        so = new UnityEditor.SerializedObject(this);
        worldManager = FindObjectOfType<WorldManager>();
        defaultSkin = (GUISkin) Resources.Load("ProvenceDefaultSkin");
    }

    public void OnEnable(){
        Selection.selectionChanged += SelectionChanged;
        SelectionChanged();
    }

    public void Initialize(){
        //Create new world manager if none are present.
        GameObject go = new GameObject("Provence Manager");
        go.tag = "ProvenceManager";
        worldManager = go.AddComponent<WorldManager>();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public void Initialize(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode){
        Debug.Log("Scene Change");
        worldManager = FindObjectOfType<WorldManager>();
    }

    void OnGUI(){
        GUI.skin = defaultSkin;
        if(!worldManager){
            if(GUILayout.Button("Initialize ProvenceECS")){
                Initialize();
            }
        }else{
            DrawWorldManager();
        }

    }

    void DrawWorldManager(){
        GUILayout.BeginHorizontal();
        if(!creatingWorld){
            foreach(World world in worldManager.worlds){
                if(world){
                    if(GUILayout.Button(world.worldName,GUILayout.Width(50),GUILayout.Height(50))){ 
                        Selection.objects = new Object[]{world.gameObject};
                    }
                }
            }

            GUILayout.FlexibleSpace();

            if(GUILayout.Button("New World",GUILayout.Width(50),GUILayout.Height(50))){
                creatingWorld = true;
            }
            
        }else{
            newWorldName = GUILayout.TextField(newWorldName,GUILayout.ExpandWidth(true),
                GUILayout.Height(50),
                GUILayout.Width(position.width - 120)
            );
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Done",GUILayout.Width(50),GUILayout.Height(50))){
                worldManager.CreateNewWorld(newWorldName);
                creatingWorld = false;
                newWorldName = "New World";
            }
            if(GUILayout.Button("X",GUILayout.Width(50),GUILayout.Height(50))){
                creatingWorld = false;
                newWorldName = "New World";
            }
        }
        GUILayout.EndHorizontal();
        if(selectedWorld && !creatingWorld){
            worldToolbarIndex = GUILayout.Toolbar(worldToolbarIndex,worldToolbarTitles);
            switch(worldToolbarIndex){
                case 0:
                    DrawEntitiesForWorld();
                    break;
            }
        }
    }

    void SelectionChanged(){
        if(Selection.objects.Length >= 1){
            var selectedObject = Selection.objects[0] as GameObject;
            if(selectedObject != null){
                switch(selectedObject.tag){
                    case "World":
                        selectedWorld = selectedObject.GetComponent<World>();
                        serializedEntityManager = new SerializedObject(selectedWorld.entityManager);
                        break;
                }
                Repaint();
            }
        }
    }

    void DrawEntitiesForWorld(){
        if(selectedWorld && serializedEntityManager != null){
            GUILayout.Label("World ID: " + selectedWorld.id, EditorStyles.boldLabel); 
            ((SerializedObject) serializedEntityManager).Update();
            GUILayout.Label("Methods: ", EditorStyles.boldLabel);  
            if(GUILayout.Button("Create Entity")){
                selectedWorld.CreateEntity();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                Selection.selectionChanged();
            }      

            SerializedProperty entityKeys = serializedEntityManager.FindProperty("entities.keys");
            SerializedProperty entityValues = serializedEntityManager.FindProperty("entities.values");

            GUILayout.Label("Available Entities: " + serializedEntityManager.FindProperty("availableEntities").arraySize, EditorStyles.boldLabel);  

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
                        selectedWorld.RemoveEntity(entityId);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel -= 1;
            ((SerializedObject) serializedEntityManager).ApplyModifiedProperties();
        }
    }

}
