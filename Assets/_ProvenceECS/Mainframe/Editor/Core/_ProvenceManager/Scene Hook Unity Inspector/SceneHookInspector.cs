using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ProvenceECS.Mainframe{
    [CustomEditor(typeof(ProvenceSceneHook))]
    public class ProvenceSceneHookInspector : MainframeUnityInspector<ProvenceSceneHook>{

        public override VisualElement CreateInspectorGUI(){
            return LoadTree(UIDirectories.GetPath("world-manager-unity-inspector","uxml"), UIDirectories.GetPath("world-manager-unity-inspector","uss"));
        }

        protected override void RegisterEventListeners(){
            root.Q<ListItemText>("manager-button").eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                ProvenceManagerEditor.ShowWindow();
            });
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            root.Q<ListItemText>("id-display").text = ((ProvenceSceneHook)target).id;
        }
    }
}