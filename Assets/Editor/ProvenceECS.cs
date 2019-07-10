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
        foreach(World world in worldManager.worlds.values){
            if(GUILayout.Button(world.name)) Debug.Log(world.name);
            worldManager.activeWorld = world;
        }
        GUILayout.EndHorizontal();
    }

}
