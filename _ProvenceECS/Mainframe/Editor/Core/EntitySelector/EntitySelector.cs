using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class EntitySelectorUIDirectory : UIDirectory{
        public EntitySelectorUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/EntitySelector/EntitySelector.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/EntitySelector/EntitySelector.uss"
            };
        }
    }

    public class EntitySelector : MainframeSelectorWindow<Entity>{

        protected World chosenWorld;
        protected Texture entityIcon;
        protected string searchString = "";

        public static void Open(World world, ProvenceDelegate<MainframeKeySelection<Entity>> callback){
            EntitySelector.Close<EntitySelector>();
            EntitySelector window = MainframeSelectorWindow<Entity>.Open<EntitySelector>("Entity Selector", callback);
            window.eventManager.Raise<SetSelectorParameters<World>>(new SetSelectorParameters<World>(world));
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Entity Selector");
            this.uiDirectory = new EntitySelectorUIDirectory();
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SetSelectorParameters<World>>(SetParameters);
            eventManager.AddListener<DrawColumnEventArgs<Entity>>(DrawColumn);
            root.Q<ListItemTextInput>("search-input").eventManager.AddListener<ListItemInputChange>(e => {
                searchString = ((ListItemTextInput)e.input).text;
                eventManager.Raise<DrawColumnEventArgs<Entity>>(new DrawColumnEventArgs<Entity>(0));
            });
            root.Q<ListItemImage>("clear-search-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                root.Q<ListItemTextInput>("search-input").value = "";
            });
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            entityIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/circle-notch.png");
        }

        protected void SetParameters(SetSelectorParameters<World> args){
            this.chosenWorld = args.paramaters;
            DrawEntityList();
        }

        protected override void DrawColumn(DrawColumnEventArgs<Entity> args){
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

        protected void DrawEntityList(){
            ColumnScroller scroller = root.Q<ColumnScroller>("list-scroller");
            if(chosenWorld != null && scroller != null){
                scroller.Clear();
                bool alternate = false;
                foreach(EntityHandle handle in chosenWorld.LookUpAllEntities()){
                    ListItem item = new ListItem(alternate,true,true);
                    item.AddIndent();
                    ListItemImage icon = item.AddImage(entityIcon);
                    icon.AddToClassList("icon");
                    
                    ComponentHandle<Name> nameHandle = handle.GetComponent<Name>();
                    if(name != null) item.AddTextDisplay(nameHandle.component.name);
                    else item.AddTextDisplay(handle.entity.ToString());

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
                            chosenKey = handle.entity;
                        }
                    });
                }
            }
        }
    }

}