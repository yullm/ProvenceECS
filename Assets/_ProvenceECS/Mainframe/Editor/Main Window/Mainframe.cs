using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ProvenceECS.Mainframe{

    public class Mainframe : MainframeTableWindow<int>{
        [MenuItem("ProvenceECS/Mainframe &1")]
        public static void ShowWindow(){
            Mainframe window = GetWindow<Mainframe>();
            window.titleContent = new GUIContent("Mainframe");
        }

        public override void OnEnable(){
            LoadTree(UIDirectories.GetPath("main","uxml"),UIDirectories.GetPath("main","uss"));
        }

        protected override void InitializeWindow(){
            CoreTableEvents();
            AdditionalTableEvents();
        }

        private void CoreTableEvents(){
            
            
        }

        private void AdditionalTableEvents(){
            root.Q<ListItem>("actor-manual-button").eventManager.AddListener<MouseClickEvent>(e => {
                EditorWindow.GetWindow<Ransacked.ActorManualEditor>();
            });            
            root.Q<ListItem>("asset-manager-button").eventManager.AddListener<MouseClickEvent>(e => {
                EditorWindow.GetWindow<AssetManagerEditor>();
            });  
        }

    }

}