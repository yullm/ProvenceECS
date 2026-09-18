using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public abstract class MainframeUnityInspector<T> : Editor where T : class{
        
        public static readonly string inspectorUss = @"Assets/_ProvenceECS/Mainframe/Editor/Core/_Base/Unity Inspector/MainframeUnityInspector.uss";

        protected T selected;
        protected VisualElement root;
        protected EventManager<MainframeUIArgs> eventManager = new EventManager<MainframeUIArgs>();
        protected UIDirectory uiDirectory;

        protected VisualElement LoadTree(){
            //root = this.
            selected = target as T;
            root = new VisualElement();

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uiDirectory.uxmlPath);
            visualTree.CloneTree(root);
    
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(MainframeTableWindow<Entity>.baseUss));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(inspectorUss));
            for(int i = 0; i < uiDirectory.ussPaths.Length; i++){
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(uiDirectory.ussPaths[i]);
                root.styleSheets.Add(styleSheet);
            }

            RegisterEventListeners();
            InitializeWindow();
            eventManager.Raise<PageLoadComplete>(new PageLoadComplete());
            return root;
        }

        protected abstract void InitializeWindow();

        protected virtual void RegisterEventListeners(){}

        protected virtual void DrawColumn(DrawColumnEventArgs<T> args){
            Debug.Log("drawing column: " + args.column);
        }
    }

}