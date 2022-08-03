using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ProvenceECS.Mainframe{

    public class SceneHookUIDirectory : UIDirectory{
        public SceneHookUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/_ProvenceManager/Scene Hook Unity Inspector/SceneHookInspector.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/_ProvenceManager/Scene Hook Unity Inspector/SceneHookInspector.uss"
            };
        }
    }

    [CustomEditor(typeof(ProvenceSceneHook))]
    public class ProvenceSceneHookInspector : MainframeUnityInspector<ProvenceSceneHook>{

        public override VisualElement CreateInspectorGUI(){
            uiDirectory = new SceneHookUIDirectory();
            return LoadTree();
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