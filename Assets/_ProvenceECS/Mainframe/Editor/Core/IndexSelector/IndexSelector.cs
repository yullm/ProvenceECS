using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class IndexSelectorUIDirectory : UIDirectory{
        public IndexSelectorUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/IndexSelector/IndexSelector.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/IndexSelector/IndexSelector.uxml"
            };
        }
    }

    public class IndexSelector : MainframeSelectorWindow<int>{

        protected int highestIndex;

        public static void Open(ProvenceDelegate<MainframeKeySelection<int>> callback, int highestIndex = -1){
            IndexSelector window = MainframeSelectorWindow<int>.Open<IndexSelector>("Index Selection", callback);
            window.highestIndex = highestIndex;
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Index Selector");
            this.uiDirectory = new IndexSelectorUIDirectory();
        }

        protected override void RegisterEventListeners(){
            if(root == null) return;
            root.Q<ListItemText>("select-button").eventManager.AddListener<MouseClickEvent>(e=>{
                if(e.button != 0) return;
                chosenKey = root.Q<ListItemIntInput>("select-input").value;
                ReturnSelection();
            });
            root.Q<ListItemText>("increment-button").eventManager.AddListener<MouseClickEvent>(e=>{
                if(e.button != 0) return;
                chosenKey = highestIndex + 1;
                ReturnSelection();
            });
        }

        protected override void InitializeWindow(){
            if(root == null) return;
        }
        
    }

}