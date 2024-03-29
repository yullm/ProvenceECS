﻿using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProvenceECS;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ProvenceECS.Mainframe{


    public class DrawColumnEventArgs<T> : MainframeUIArgs{
        public int column;
        public T key;
        public DrawColumnEventArgs(int column){
            this.column = column;
        }

        public DrawColumnEventArgs(int column, T key){
            this.column = column;
            this.key = key;
        }
    }

    public class PageLoadComplete : MainframeUIArgs{}

    public class SelectKey<T> : MainframeUIArgs{
        public T key;

        public SelectKey(T key){
            this.key = key;
        }
    }

    public class ElementUpdated : MainframeUIArgs{
        public VisualElement target;

        public ElementUpdated(VisualElement target){
            this.target = target;
        }
    }

    //Scene Events

    public class SceneSavedEvent : MainframeUIArgs{
        public Scene scene;

        public SceneSavedEvent(Scene scene){
            this.scene = scene;
        }
    }

    public class SceneLoadedEvent : MainframeUIArgs{
        public Scene scene;

        public SceneLoadedEvent(Scene scene){
            this.scene = scene;
        }
    }

    public class SetSceneDirtyEvent : MainframeUIArgs{
        public Scene scene;

        public SetSceneDirtyEvent(Scene scene){
            this.scene = scene;
        }
    }

    public class UIDirectory{
        public static readonly string provenceEditorRoot = @"Assets/_ProvenceECS/Mainframe/Editor";
        public string uxmlPath;
        public string[] ussPaths;

        public UIDirectory(){
            this.uxmlPath = "";
            this.ussPaths = new string[0];
        }
    }

    public abstract class MainframeTableWindow<T> : EditorWindow{

        public static readonly string baseUss = @"Assets/_ProvenceECS/Mainframe/Editor/Core/_Base/MainframeTable.uss";

        protected UIDirectory uiDirectory;
        
        protected VisualElement root;
        protected T chosenKey;
        protected GUIStyle previewStyle;
        public EventManager<MainframeUIArgs> eventManager = new EventManager<MainframeUIArgs>();
        protected PlayModeStateChange playModeState;

        public MainframeTableWindow(){
            EditorApplication.playModeStateChanged += (state) =>{
                playModeState = state;
                EditorStateChange(state);
            };

            uiDirectory = new UIDirectory();
        }

        /// <summary>
        ///     Set Title and uiKey
        /// </summary>
        protected abstract void SetEditorSettings();

        public virtual void OnEnable(){
            SetEditorSettings();
            LoadTree();
        }

        protected virtual void LoadTree(){
            root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uiDirectory.uxmlPath);
            VisualElement tree = visualTree.CloneTree();
            root.Add(tree);
            
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(baseUss));
            for(int i = 0; i < uiDirectory.ussPaths.Length; i++){
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(uiDirectory.ussPaths[i]);
                root.styleSheets.Add(styleSheet);
            }
            
            SetReferences();
            RegisterSceneEvents();
            RegisterEventListeners();
            InitializeWindow();
            eventManager.Raise<PageLoadComplete>(new PageLoadComplete());
        }

        protected virtual void SetReferences(){

        }

        protected void RegisterSceneEvents(){
            EditorSceneManager.sceneSaved += scene =>{
                eventManager.Raise<SceneSavedEvent>(new SceneSavedEvent(scene));
            };
            EditorSceneManager.sceneOpened += (scene, mode) =>{
                eventManager.Raise<SceneLoadedEvent>(new SceneLoadedEvent(scene));
            };
            eventManager.AddListener<SetSceneDirtyEvent>(SetSceneDirty);

            AssemblyReloadEvents.beforeAssemblyReload += () =>{
                BeforeAssemblyReload();
            };
            AssemblyReloadEvents.afterAssemblyReload += () =>{
                AfterAssemblyReload();
            };            
        }

        protected virtual void RegisterEventListeners(){}

        protected abstract void InitializeWindow();
        
        protected virtual void DrawColumn(DrawColumnEventArgs<T> args){
            Debug.Log("drawing column: " + args.column);
        }

        public static void Close<M>() where M : MainframeTableWindow<T>{
            if(EditorWindow.HasOpenInstances<M>()) GetWindow<M>().Close();
        }

        protected void SetSceneDirty(SetSceneDirtyEvent args){
            if(!Application.isPlaying) EditorSceneManager.MarkSceneDirty(args.scene);
        }

        protected virtual void BeforeAssemblyReload(){}

        protected virtual void AfterAssemblyReload(){}

        protected virtual void EditorStateChange(PlayModeStateChange args){}

        //Data Table Methods

        protected void InitializeObjectField(ObjectField objectField, bool allowScene, System.Type objectType, EventCallback<ChangeEvent<Object>> onChangeCallback = null){
            objectField.allowSceneObjects = allowScene;
            objectField.objectType = objectType;
            if(onChangeCallback != null) objectField.RegisterValueChangedCallback(onChangeCallback);
        }

        protected virtual void SelectKey(SelectKey<T> args){
            chosenKey = args.key;
        }

        protected void DrawObjectPreview(IMGUIContainer wrapper, ref Editor gameObjectEditor, GameObject gameObject){
            if(previewStyle == null){ 
                previewStyle = new GUIStyle();
            }
            if(wrapper != null && gameObject != null){
                if(gameObjectEditor == null || gameObjectEditor.target != gameObject) gameObjectEditor = Editor.CreateEditor(gameObject);
                gameObjectEditor.OnInteractivePreviewGUI(wrapper.contentRect, previewStyle);
            }
            
        }

        protected ListItem DrawShelf(string titleText, out Div container){
            ListItem titleItem = new ListItem();
            titleItem.AddToClassList("spacer","selectable","container-title");
            ListItemText title = titleItem.AddTitle(titleText);
            container = new Div();
            container.AddToClassList("category-container");
            
            Div containerRef = container;
            titleItem.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                else title.AddToClassList("second-alternate");
                containerRef.Toggle();
            });
            return titleItem; 
        }
    }
}