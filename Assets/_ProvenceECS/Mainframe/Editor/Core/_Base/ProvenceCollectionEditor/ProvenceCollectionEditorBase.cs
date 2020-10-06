using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using ProvenceECS.Mainframe;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public abstract class ProvenceCollectionEditorBase<T,U> : MainframeTableWindow<string>  where T : ProvenceCollection<U>, new() where U : ProvenceCollectionEntry, new (){

        #region CollectionEditor variables
        protected T collection = new T();
        
        protected ColumnScroller keyListScroller;
        protected ColumnScroller entryEditorScroller;
        protected Texture keyIcon;
        protected Texture selectedKeyIcon;
        protected DropDownMenu keyListContextMenu;
        protected UnityEditor.Editor modelViewerEditor;
        protected GameObject previewModel;
        #endregion
        
        #region CollectionEditor windowMethods
        public override void OnEnable(){
            LoadCollection();
            SetEditorSettings();
            LoadTree(UIDirectories.GetPath(uiKey,"uxml"), UIDirectories.GetPath("base","uss"), UIDirectories.GetPath("collection-editor","uss"), UIDirectories.GetPath(uiKey,"uss"));
            modelViewerEditor = null;
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SelectKey<string>>(SelectKey);
            eventManager.AddListener<DrawColumnEventArgs<string>>(DrawColumn);
            eventManager.AddListener<SceneSavedEvent>(SaveManual);
            RegisterKeyListEvents();
            RegisterEntryEditorEvents();
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            keyListScroller = root.Q<ColumnScroller>("key-list-scroller");
            entryEditorScroller = root.Q<ColumnScroller>("entry-editor-scroller");
            keyListContextMenu = root.Q<DropDownMenu>("key-list-context-menu");
            keyIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/bookmark-open.png");
            selectedKeyIcon = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/bookmark.png");
            eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
        }

        protected override void SelectKey(SelectKey<string> args){
            chosenKey = args.key;
            eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
        }

        protected override void DrawColumn(DrawColumnEventArgs<string> args){
            switch(args.column){
                case 0:
                    DrawKeyList();
                    break;
                case 1:
                    DrawEntryEditor();
                    break;
            }
        } 
        #endregion

        #region CollectionEditor keyListMethods
        protected virtual void RegisterKeyListEvents(){
            ListItemTextInput keyInput = root.Q<ListItemTextInput>("new-entry-key-input");
            keyInput.eventManager.AddListener<ListItemInputCommit>(e => {
                AddManualEntry(keyInput.value);
                keyInput.value = "";
            });
            root.Q<ListItemImage>("new-entry-add-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                AddManualEntry(keyInput.value);
                keyInput.value = "";
            });
            root.Q<ListItemImage>("new-entry-key-input-clear").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                keyInput.value = "";
            });

            ListItemTextInput searchInput = root.Q<ListItemTextInput>("key-list-search-input");
            searchInput.eventManager.AddListener<ListItemInputChange>(e => {
                SearchKeyList(searchInput.text);
            });

            root.Q<ListItemImage>("key-list-search-clear").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                searchInput.value = "";
                SearchKeyList(searchInput.text);
            });
        }

        protected virtual void DrawKeyList(){
            keyListScroller.Clear();
            bool alternate = false;
            collection.Sort();
            foreach(string key in collection.Keys){
                ListItem item = new ListItem(alternate,true,true);
                item.userData = key;
                item.AddIndent(true);
                
                ListItemImage icon = item.AddImage(keyIcon);
                icon.AddToClassList("icon");
                ListItemTextInput nameInput = item.AddTextField(key);
                nameInput.AddToClassList("key-list-name-input");
                ListItemText nameDisplay = item.AddTextDisplay(key);

                if(chosenKey == key){
                    item.AddToClassList("selected");
                    item.Q<ListItemImage>().SetImage(selectedKeyIcon);
                }

                item.eventManager.AddListener<MouseClickEvent>(e => {
                    switch(e.button){
                        case 0:
                            if(!item.ClassListContains("selected")){    

                                ListItem selected = keyListScroller.Q<ListItem>(null,"selected");
                                if(selected != null){
                                    selected.RemoveFromClassList("selected");
                                    selected.Q<ListItemImage>().SetImage(keyIcon);
                                }
                                item.AddToClassList("selected");
                                icon.SetImage(selectedKeyIcon);
                                
                                eventManager.Raise<SelectKey<string>>(new SelectKey<string>(key));
                            }
                            break;
                        case 1:
                            keyListContextMenu.Show(root,e,true);
                            ListItem editButton = keyListContextMenu.Q<ListItem>("key-list-context-menu-edit-button");
                            editButton.eventManager.ClearListeners();
                            editButton.eventManager.AddListener<MouseClickEvent>(ev=>{
                                if(ev.button != 0) return;
                                nameInput.style.display = DisplayStyle.Flex;
                                nameDisplay.style.display = DisplayStyle.None;
                                keyListContextMenu.style.display = DisplayStyle.None;
                            });

                            ListItem removeButton = keyListContextMenu.Q<ListItem>("key-list-context-menu-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev=>{
                                if(ev.button != 0) return;
                                RemoveManualEntry(key);
                                keyListContextMenu.style.display = DisplayStyle.None;
                            });
                            break;
                    }
                });

                nameInput.eventManager.AddListener<ListItemInputCommit>(e =>{
                    EditManualEntryKey(key,nameInput.value);
                });

                nameInput.eventManager.AddListener<ListItemInputCancel>(e =>{
                    nameInput.value = key;
                    nameInput.style.display = DisplayStyle.None;
                    nameDisplay.style.display = DisplayStyle.Flex;
                });

                keyListScroller.Add(item);
                alternate = !alternate;
            }
            //Scroll to selected if exists
        }

        protected virtual void SearchKeyList(string searchString){

            keyListScroller.Query<ListItem>().ForEach(item => {
                item.style.display = DisplayStyle.Flex;
            });

            if(!searchString.Equals("")){
                keyListScroller.Query<ListItem>().ForEach(item => {
                    bool match = false;
                    string itemKey = ((string)item.userData);
                    if(collection.ContainsKey(itemKey)){
                        U entry = collection[itemKey];
                        if(entry.name.ToLower().Contains(searchString.ToLower())) match = true;
                        if(match == false){
                            foreach(string tag in entry.tags){
                                if(tag.ToLower().Contains(searchString.ToLower())) match = true;
                            }
                            if(match == false) item.style.display = DisplayStyle.None;
                        }
                    }
                });
            }
        }

        #endregion

        #region CollectionEditor entryEditorMethods

        protected virtual void RegisterEntryEditorEvents(){}

        protected virtual bool DrawEntryEditor(){
            entryEditorScroller.Clear();
            if(chosenKey != null){
                root.Q<ListItem>("entry-editor-title").style.display = DisplayStyle.Flex;
                root.Q<ListItem>("entry-editor-menu").style.display = DisplayStyle.Flex;
                root.Q<ListItemText>("entry-editor-title-display").text = chosenKey;
                return true;
            }else{
                root.Q<ListItem>("entry-editor-title").style.display = DisplayStyle.None; 
                root.Q<ListItem>("entry-editor-menu").style.display = DisplayStyle.None;
                return false;
            }
        }

        #endregion       

        #region  CollectionEditor manualMethods
        protected virtual void AddManualEntry(string name){
            if(!collection.ContainsKey(name)){
                collection[name] = new U();
                collection[name].name = name;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<SelectKey<string>>(new SelectKey<string>(name));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
            }
        }

        protected virtual void EditManualEntryKey(string key, string name){
            if(collection.ContainsKey(key) && !collection.ContainsKey(name)){
                collection[name] = collection[key];
                collection.Remove(key);
                collection[name].name = name;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<SelectKey<string>>(new SelectKey<string>(name));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
                
            }
        }

        protected virtual void RemoveManualEntry(string key){
            if(collection.ContainsKey(key)){
                if(chosenKey == key) eventManager.Raise<SelectKey<string>>(new SelectKey<string>(null));
                collection.Remove(key);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
            }
        }
        
        protected abstract void LoadCollection();

        protected void SaveManual(SceneSavedEvent args){
            collection.Save();
        }
        #endregion

    }

}