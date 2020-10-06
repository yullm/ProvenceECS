using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class KeySelector : MainframeSelectorWindow<string>{

        protected HashSet<string> set;
        protected string searchString = "";

        public static KeySelector Open(){
            KeySelector.Close<KeySelector>();
            return KeySelector.GetWindow<KeySelector>();
        }

        public override void OnEnable(){
            SetEditorSettings();
            LoadTree(UIDirectories.GetPath(uiKey,"uxml"), UIDirectories.GetPath(uiKey,"uss"));
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Key Selector");
            this.uiKey = "key-selector";
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SetSelectorParameters<HashSet<string>>>(SetParameters);
            eventManager.AddListener<DrawColumnEventArgs<string>>(DrawColumn);
            root.Q<ListItemTextInput>("search-input").eventManager.AddListener<ListItemInputChange>(e => {
                searchString = ((ListItemTextInput)e.input).text;
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
            });
            root.Q<ListItemImage>("clear-search-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                root.Q<ListItemTextInput>("search-input").value = "";
            });
        }

        protected override void InitializeWindow(){
            if(root == null) return;
        }

        protected void SetParameters(SetSelectorParameters<HashSet<string>> args){
            set = args.paramaters;
            DrawKeyList();
        }        

        protected override void DrawColumn(DrawColumnEventArgs<string> args){
            switch(args.column){
                case 0:
                    SortListBySearch();
                    break;
            }
        }

        protected void SortListBySearch(){
            ColumnScroller scroller = root.Q<ColumnScroller>("list-scroller");
            scroller.Query<ListItem>().ForEach(item => {
                item.style.display = DisplayStyle.Flex;
            });

            if(!searchString.Equals("")){
                scroller.Query<ListItem>().ForEach(item => {
                    if(!item.Q<ListItemText>().text.ToLower().Contains(searchString.ToLower())) item.style.display = DisplayStyle.None; 
                });
            }
        }

        protected void DrawKeyList(){
            ColumnScroller scroller = root.Q<ColumnScroller>("list-scroller");
            if(scroller != null){
                scroller.Clear();
                bool alternate = false;
                foreach(string key in set){
                    ListItem item = new ListItem(alternate,true,true);
                    item.AddIndent();
                    /* ListItemImage icon = item.AddImage(entityIcon);
                    icon.AddToClassList("icon"); */
                    
                    item.AddTitle(key);

                    scroller.Add(item);
                    alternate = !alternate;

                    item.eventManager.AddListener<MouseClickEvent>(e =>{
                        if(e.button != 0) return;
                        if(item.ClassListContains("selected")){
                            ReturnSelection();
                        }else{
                            scroller.Query<ListItem>(null,"selected").ForEach(current =>{
                                current.RemoveFromClassList("selected");
                            });
                            item.AddToClassList("selected");
                            chosenKey = key;
                        }
                    });
                }
            }
        }
    }
}