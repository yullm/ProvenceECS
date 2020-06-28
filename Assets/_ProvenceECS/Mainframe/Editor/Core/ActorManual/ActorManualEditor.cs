using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using ProvenceECS.Mainframe;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Ransacked{

    public class ActorManualEditor : MainframeTableWindow<string>{

        #region ActorManual variables
        protected ActorManual actorManual = new ActorManual();
        protected ColumnScroller keyListScroller;
        protected ColumnScroller entryEditorScroller;
        protected Texture keyIcon;
        protected Texture selectedKeyIcon;
        protected DropDownMenu keyListContextMenu;
        protected DropDownMenu EntryEditorComponentContextMenu;
        protected UnityEditor.Editor modelViewerEditor;
        protected GameObject previewModel;
        #endregion
        
        #region ActorManual windowMethods
        public override void OnEnable(){
            this.titleContent = new GUIContent("Actor Manual");
            LoadManual();
            LoadTree(UIDirectories.GetPath("actor-manual","uxml"), UIDirectories.GetPath("actor-manual","uss"));
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
            EntryEditorComponentContextMenu = root.Q<DropDownMenu>("entry-editor-component-context-menu");
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

        #region ActorManual keyListMethods
        protected void RegisterKeyListEvents(){
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

        protected void DrawKeyList(){
            keyListScroller.Clear();
            bool alternate = false;
            actorManual.Sort();
            foreach(string key in actorManual.Keys){
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

        protected void SearchKeyList(string searchString){

            keyListScroller.Query<ListItem>().ForEach(item => {
                item.style.display = DisplayStyle.Flex;
            });

            if(!searchString.Equals("")){
                keyListScroller.Query<ListItem>().ForEach(item => {
                    bool match = false;
                    string itemKey = ((string)item.userData);
                    if(actorManual.ContainsKey(itemKey)){
                        ActorManualEntry entry = actorManual[itemKey];
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

        #region ActorManual entryEditorMethods

        protected void RegisterEntryEditorEvents(){
            root.Q<Div>("entry-editor-menu-add-component-button").eventManager.AddListener<MouseClickEvent>(AddComponentButtonClicked);
        }

        protected void AddComponentButtonClicked(MouseClickEvent e){
            if(e.button != 0) return;
            if(actorManual.ContainsKey(chosenKey)){
                List<Type> existingTypes = actorManual[chosenKey].components.Keys.ToList();
                existingTypes.Add(typeof(ActorManualInstance),typeof(Actor));
                TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceComponent), false, existingTypes, typeof(Entity));
                TypeSelector.Open(searchParameters, args =>{
                    Helpers.InvokeGenericMethod<ActorManualEditor>(this,"AddComponentToEntry", args.value);
                });
            }
        }

        protected void DrawEntryEditor(){
            entryEditorScroller.Clear();
            if(chosenKey != null){
                root.Q<ListItem>("entry-editor-title").style.display = DisplayStyle.Flex;
                root.Q<ListItem>("entry-editor-menu").style.display = DisplayStyle.Flex;
                root.Q<ListItemText>("entry-editor-title-display").text = chosenKey;                
                DrawComponent(actorManual[chosenKey].actorComponent, entryEditorScroller);
                DrawUncategorizedComponents();
                DrawModelViewer();
            }else{
                root.Q<ListItem>("entry-editor-title").style.display = DisplayStyle.None; 
                root.Q<ListItem>("entry-editor-menu").style.display = DisplayStyle.None;
            }
        }

        protected void DrawUncategorizedComponents(){
            if(actorManual.ContainsKey(chosenKey)){
                ListItem titleItem = new ListItem();
                titleItem.AddToClassList("spacer","selectable");
                ListItemText title = titleItem.AddTitle("Uncategorized Components");                
                Div container = new Div();
                container.AddToClassList("category-container");

                titleItem.eventManager.AddListener<MouseClickEvent>(e =>{
                    if(e.button != 0) return;
                    if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                    else title.AddToClassList("second-alternate");
                    container.Toggle();
                });                

                foreach(ProvenceComponent component in actorManual[chosenKey].uncategorizedComponents)
                    DrawComponent(component,container);

                entryEditorScroller.Add(titleItem,container);
            }
        }

        protected void DrawComponent(ProvenceComponent component, VisualElement parent){
            ListItem titleItem = new ListItem(false,true);
            ListItemText title = titleItem.AddTitle(System.Text.RegularExpressions.Regex.Replace(component.GetType().Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));
            titleItem.name = component.GetType().Name.ToLower() + "-control-title";
            titleItem.AddToClassList("structure-control-title","spacer");

            Div structContainer = (Div)Helpers.InvokeGenericMethod<ActorManualEditor>(this,"DrawComponentControl",component.GetType(),component);
            
            titleItem.eventManager.AddListener<MouseClickEvent>(e =>{
                switch(e.button){
                    case 0:
                        if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                        else title.AddToClassList("second-alternate");
                        structContainer.Toggle();
                        break;
                    case 1:
                        if(component.GetType() != typeof(Actor)){
                            EntryEditorComponentContextMenu.Show(root,e,true);

                            ListItem removeButton = EntryEditorComponentContextMenu.Q<ListItem>("entry-editor-component-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                Helpers.InvokeGenericMethod<ActorManualEditor>(this, "RemoveComponentFromEntry", component.GetType());
                                EntryEditorComponentContextMenu.Toggle();
                            });

                            ListItem moveupButton = EntryEditorComponentContextMenu.Q<ListItem>("entry-editor-component-moveup-button");
                            moveupButton.eventManager.ClearListeners();
                            moveupButton.eventManager.AddListener<MouseClickEvent>(ev =>{
                                MoveUpComponentInList(component);
                                EntryEditorComponentContextMenu.Toggle();
                            });

                            ListItem movedownButton = EntryEditorComponentContextMenu.Q<ListItem>("entry-editor-component-movedown-button");
                            movedownButton.eventManager.ClearListeners();
                            movedownButton.eventManager.AddListener<MouseClickEvent>(ev =>{
                                MoveDownComponentInList(component);
                                EntryEditorComponentContextMenu.Toggle();
                            });
                        }
                        break;
                }
            });

            parent.Add(titleItem);
            parent.Add(structContainer);
        }

        protected Div DrawComponentControl<T>(T component) where T : ProvenceComponent{
            Div container = new Div();
            container.name = typeof(T).Name.ToLower() + "-control-container";

            StructureControl<T> control = new StructureControl<T>(ref component,null,false,null,null,0,typeof(DontDisplayInManual));
            control.eventManager.AddListener<StructureControlUpdated<T>>(e =>{
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });

            container.Add(control);
            return container;
        }

        protected void DrawModelViewer(){
            PageColumn modelViewer = root.Q<PageColumn>("model-viewer");
            modelViewer.Clear();
            if(modelViewer != null){
                if(actorManual.ContainsKey(chosenKey) && actorManual[chosenKey].components.ContainsKey(typeof(Model))){
                    Model modelComponent = (Model)actorManual[chosenKey].components[typeof(Model)];
                    if(modelComponent.address.Equals("")) return;
                    previewModel = ProvenceManager.AssetManager.LoadAsset<GameObject>(modelComponent.address);
                    if(previewModel == null) return;
                    modelViewer.style.display = DisplayStyle.Flex;                    
                    IMGUIContainer modelViewerWrapper = new IMGUIContainer();
                    modelViewerWrapper.AddToClassList("model-preview-wrapper");
                    modelViewer.Add(modelViewerWrapper);
                    modelViewerWrapper.onGUIHandler = () =>{
                        DrawObjectPreview(modelViewerWrapper, ref modelViewerEditor, previewModel);
                    };
                }else modelViewer.style.display = DisplayStyle.None;
            }
        }

        #endregion       

        #region  ActorManual manualMethods
        protected void AddManualEntry(string name){
            if(!actorManual.ContainsKey(name)){
                actorManual[name] = new ActorManualEntry(name);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<SelectKey<string>>(new SelectKey<string>(name));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
            }
        }

        protected void EditManualEntryKey(string key, string name){
            if(actorManual.ContainsKey(key) && !actorManual.ContainsKey(name)){
                actorManual[name] = actorManual[key];
                actorManual.Remove(key);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<SelectKey<string>>(new SelectKey<string>(name));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
                
            }
        }

        protected void RemoveManualEntry(string key){
            if(actorManual.ContainsKey(key)){
                if(chosenKey == key) eventManager.Raise<SelectKey<string>>(new SelectKey<string>(null));
                actorManual.Remove(key);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
            }
        }
        
        protected void AddComponentToEntry<T>() where T : ProvenceComponent, new(){
            if(actorManual.ContainsKey(chosenKey) && !actorManual[chosenKey].components.ContainsKey(typeof(T))){
                T component = new T();
                actorManual[chosenKey].components[typeof(T)] = component;
                actorManual[chosenKey].uncategorizedComponents.Add(component);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected void RemoveComponentFromEntry<T>() where T : ProvenceComponent{
            if(actorManual.ContainsKey(chosenKey) && actorManual[chosenKey].components.ContainsKey(typeof(T))){
                ProvenceComponent component = actorManual[chosenKey].components[typeof(T)];
                if(actorManual[chosenKey].uncategorizedComponents.Contains(component)) actorManual[chosenKey].uncategorizedComponents.Remove(component);
                else{
                    foreach(List<ProvenceComponent> list in actorManual[chosenKey].categorizedComponents.Values){
                        if(list.Contains(component)){
                            list.Remove(component);
                            break;
                        }
                    }
                }
                actorManual[chosenKey].components.Remove(typeof(T));
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected void MoveUpComponentInList(ProvenceComponent component){
            List<ProvenceComponent> list = null;
            if(actorManual.ContainsKey(chosenKey)){
                if(actorManual[chosenKey].uncategorizedComponents.Contains(component))
                    list = actorManual[chosenKey].uncategorizedComponents;
                else{
                    foreach(List<ProvenceComponent> compList in actorManual[chosenKey].categorizedComponents.Values){
                        if(compList.Contains(component)){
                            list = compList;
                            break;
                        }
                    }
                }
                int index = list.IndexOf(component);
                if(index != 0){
                    list.Swap(index,index - 1);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
                }
            }
        }

        protected void MoveDownComponentInList(ProvenceComponent component){
            List<ProvenceComponent> list = null;
            if(actorManual.ContainsKey(chosenKey)){
                if(actorManual[chosenKey].uncategorizedComponents.Contains(component))
                    list = actorManual[chosenKey].uncategorizedComponents;
                else{
                    foreach(List<ProvenceComponent> compList in actorManual[chosenKey].categorizedComponents.Values){
                        if(compList.Contains(component)){
                            list = compList;
                            break;
                        }
                    }
                }
                int index = list.IndexOf(component);
                if(index < list.Count - 1){
                    list.Swap(index,index + 1);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
                }
            }
        }

        protected void LoadManual(){
            actorManual = ProvenceManager.ActorManual;
        }

        protected void SaveManual(SceneSavedEvent args){
            actorManual.Save();
        }
        #endregion

    }

}