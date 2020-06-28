using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public abstract class MainframeUnityInspector<T> : Editor where T : class{
        
        protected T selected;
        protected VisualElement root;
        protected EventManager<MainframeUIArgs> eventManager = new EventManager<MainframeUIArgs>();

        protected VisualElement LoadTree(string xmlPath, params string[] ussPaths){
            selected = target as T;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(xmlPath);
            root = visualTree.CloneTree();
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(UIDirectories.GetPath("base","uss")));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(UIDirectories.GetPath("base","inspector-uss")));
            for(int i = 0; i < ussPaths.Length; i++){
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPaths[i]);
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