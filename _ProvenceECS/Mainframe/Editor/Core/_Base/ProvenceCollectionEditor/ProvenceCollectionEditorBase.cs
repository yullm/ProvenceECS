using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using ProvenceECS.Mainframe;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor;

namespace ProvenceECS.Mainframe{

    public abstract class ProvenceCollectionEditor<U> : MainframeTableWindow<string> where U : ProvenceCollectionEntry, new (){

        #region ManualEditor variables
        protected ProvenceCollection<U> collection = new ProvenceCollection<U>();
        public static readonly string collectionUss = @"Assets/_ProvenceECS/Mainframe/Editor/Core/_Base/ProvenceCollectionEditor/ProvenceCollectionEditorBase.uss";
        
        protected ColumnScroller keyListScroller;
        protected ColumnScroller entryEditorScroller;
        protected ColumnScroller tagListScroller;
        protected Texture keyIcon;
        protected Texture selectedKeyIcon;
        protected DropDownMenu keyListContextMenu;
        protected DropDownMenu entryEditorComponentContextMenu;
        protected DropDownMenu entryEditorTagContextMenu;
        protected UnityEditor.Editor modelViewerEditor;
        protected GameObject previewModel;
        #endregion
        
        #region ManualEditor windowMethods
        public override void OnEnable(){
            LoadCollection();
            SetEditorSettings();
            LoadTree();
            modelViewerEditor = null;
        }

        public void OnDisable(){
            DestroyImmediate(modelViewerEditor);
        }

        protected override void LoadTree(){
            root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uiDirectory.uxmlPath);
            VisualElement tree = visualTree.CloneTree();
            root.Add(tree);
            
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(baseUss));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(collectionUss));
            for(int i = 0; i < uiDirectory.ussPaths.Length; i++){
                StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(uiDirectory.ussPaths[i]);
                root.styleSheets.Add(styleSheet);
            }
            
            RegisterSceneEvents();
            RegisterEventListeners();
            InitializeWindow();
            eventManager.Raise<PageLoadComplete>(new PageLoadComplete());
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
            root.Q<ListItemSearchBar>("entry-editor-search-bar").container = entryEditorScroller;
            tagListScroller = root.Q<ColumnScroller>("entry-editor-tag-scroller");

            keyListContextMenu = root.Q<DropDownMenu>("key-list-context-menu");
            entryEditorComponentContextMenu = root.Q<DropDownMenu>("entry-editor-component-context-menu");
            entryEditorTagContextMenu = root.Q<DropDownMenu>("entry-editor-tag-context-menu");
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

        #region ManualEditor keyListMethods
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
                            foreach(Type tag in entry.tags){
                                if(tag.Name.ToLower().Contains(searchString.ToLower())) match = true;
                            }
                            if(match == false) item.style.display = DisplayStyle.None;
                        }
                    }
                });
            }
        }

        #endregion

        #region Manual entryEditorMethods

        protected virtual void RegisterEntryEditorEvents(){
            root.Q<Div>("entry-editor-menu-add-component-button").eventManager.AddListener<MouseClickEvent>(AddComponentButtonClicked);
            root.Q<ListItemImage>("entry-editor-tag-button").eventManager.AddListener<MouseClickEvent>(AddTagButtonClicked);
        }

        protected virtual void AddComponentButtonClicked(MouseClickEvent e){
            if(e.button != 0) return;
            if(collection.ContainsKey(chosenKey)){
                List<Type> existingTypes = collection[chosenKey].components.Keys.ToList();
                TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceComponent), false, existingTypes, typeof(Entity));
                TypeSelector.Open(searchParameters, args =>{
                    Helpers.InvokeGenericMethod<ProvenceCollectionEditor<U>>(this,"AddComponentToEntry", args.value);
                });
            }
        }

        protected virtual void AddTagButtonClicked(MouseClickEvent e){
            if(e.button != 0) return;
            if(collection.ContainsKey(chosenKey)){
                List<Type> existingTypes = collection[chosenKey].tags.Select(t => t.GetType()).ToList();
                TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(MainframeTag), false, existingTypes);
                TypeSelector.Open(searchParameters, args =>{
                    AddTagToEntry(args.value);
                });
            }
        }

        protected virtual void DrawEntryEditor(){
            entryEditorScroller.Clear();
            DrawEntryTitle();
            DrawComponentList();
            DrawTagList();
            VerifyTagComponents();
            DrawModelViewer();
        }

        protected virtual void DrawEntryTitle(){
            if(chosenKey != null){
                root.Q<PageColumn>("entry-editor").style.display = DisplayStyle.Flex;
                root.Q<ListItemText>("entry-editor-title-display").text = chosenKey;
            }else{
                root.Q<PageColumn>("entry-editor").style.display = DisplayStyle.None; 
            }
        }

        protected virtual void DrawComponentList(){
            if(chosenKey != null){
                foreach(ProvenceComponent component in collection[chosenKey].components.Values){
                    DrawComponent(component, entryEditorScroller);
                }
            }
        }

        protected virtual void DrawComponent(ProvenceComponent component, VisualElement parent, bool removeable = true, bool hide = false){
            Div container = new Div();
            container.AddToClassList("search-list-item");
            container.userData = component.GetType().Name;
            foreach(Type tag in collection[chosenKey].tags){
                if(((MainframeTag)Activator.CreateInstance(tag)).requiredComponents.Contains(component.GetType()))
                    container.userData += tag.Name;
            }
            ListItem titleItem = new ListItem(false,true);
            ListItemText title = titleItem.AddTitle(System.Text.RegularExpressions.Regex.Replace(component.GetType().Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));
            titleItem.name = component.GetType().Name.ToLower() + "-control-title";
            titleItem.AddToClassList("structure-control-title","spacer");

            Div structContainer = (Div)Helpers.InvokeGenericMethod<ProvenceCollectionEditor<U>>(this,"DrawComponentControl",component.GetType(),component);
            
            titleItem.eventManager.AddListener<MouseClickEvent>(e =>{
                switch(e.button){
                    case 0:
                        if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                        else title.AddToClassList("second-alternate");
                        structContainer.Toggle();
                        break;
                    case 1:
                        if(removeable){
                            entryEditorComponentContextMenu.Show(root,e,true);

                            ListItem removeButton = entryEditorComponentContextMenu.Q<ListItem>("entry-editor-component-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                Helpers.InvokeGenericMethod<ProvenceCollectionEditor<U>>(this, "RemoveComponentFromEntry", component.GetType());
                                entryEditorComponentContextMenu.Toggle();
                            });
                        }
                        break;
                }
            });
            
            if(hide) titleItem.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(titleItem,0,Vector2.zero,null));
            
            container.Add(titleItem,structContainer);
            parent.Add(container);
        }

        protected virtual Div DrawComponentControl<V>(V component) where V : ProvenceComponent{
            Div container = new Div();
            container.name = typeof(V).Name.ToLower() + "-control-container";

            StructureControl<V> control = new StructureControl<V>(ref component,null,false,null,null,0,typeof(DontDisplayInManual));
            control.eventManager.AddListener<StructureControlUpdated<V>>(e =>{
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });

            container.Add(control);
            return container;
        }

        protected virtual void DrawTagList(){
            tagListScroller.Clear();
            if(chosenKey != null){
                foreach(System.Type tag in collection[chosenKey].tags){
                    ListItemText tagElement = new ListItemText(tag.Name);
                    tagElement.userData = tag;
                    tagElement.AddToClassList("entry-editor-tag");

                    tagElement.eventManager.AddListener<MouseClickEvent>(e =>{
                        switch(e.button){
                            case 0:
                                ListItemSearchBar compSearchBar = root.Q<ListItemSearchBar>("entry-editor-search-bar");
                                compSearchBar.searchInput.value = compSearchBar.searchInput.value.Replace(" ","");
                                if(tagElement.ClassListContains("selected")){
                                    tagElement.RemoveFromClassList("selected");
                                    compSearchBar.searchInput.value = compSearchBar.searchInput.text.ToLower().Replace(tag.Name.ToLower() + "/","");
                                }else{
                                    tagElement.AddToClassList("selected");
                                    if(compSearchBar.searchInput.value == ""){ 
                                        compSearchBar.searchInput.value = tag.Name.ToLower() + "/";
                                    }else{
                                        if(compSearchBar.searchInput.value[compSearchBar.searchInput.value.Length -1] != '/')
                                            compSearchBar.searchInput.value += "/";
                                        compSearchBar.searchInput.value += tag.Name.ToLower() + "/";
                                    }
                                }
                                break;
                            case 1:
                                entryEditorTagContextMenu.Show(root,e,true);
                                ListItem removeButton = entryEditorTagContextMenu.Q<ListItem>("entry-editor-tag-remove-button");
                                removeButton.eventManager.ClearListeners();
                                removeButton.eventManager.AddListener<MouseClickEvent>(ev =>{
                                    RemoveTagFromEntry((dynamic)tagElement.userData);
                                    entryEditorTagContextMenu.Toggle();
                                });
                                ListItem removeAllButton = entryEditorTagContextMenu.Q<ListItem>("entry-editor-tag-remove-all-button");
                                removeAllButton.eventManager.ClearListeners();
                                removeAllButton.eventManager.AddListener<MouseClickEvent>(ev =>{
                                    RemoveTagCompleteFromEntry((dynamic)tagElement.userData);
                                    entryEditorTagContextMenu.Toggle();
                                });
                                break;
                        }
                    });

                    tagListScroller.Add(tagElement);
                }
            }
        }

        protected virtual void VerifyTagComponents(){
            if(chosenKey != null){
                foreach(Type tag in collection[chosenKey].tags){
                    foreach(System.Type requiredType in ((MainframeTag)Activator.CreateInstance(tag)).requiredComponents){
                        if(!collection[chosenKey].components.ContainsKey(requiredType)){
                            Helpers.InvokeGenericMethod(this,"AddComponentToEntry", requiredType);
                        }
                    }
                }
            }
        }

        protected virtual void DrawModelViewer(){
            PageColumn modelViewer = root.Q<PageColumn>("model-viewer");
            DestroyImmediate(modelViewerEditor);
            modelViewer.Clear();
            if(chosenKey != null && modelViewer != null){
                if(collection.ContainsKey(chosenKey) && collection[chosenKey].components.ContainsKey(typeof(Model))){
                    Model modelComponent = (Model)collection[chosenKey].components[typeof(Model)];
                    if(modelComponent.manualKey.Equals("")){
                        modelViewer.style.display = DisplayStyle.None;
                        return;
                    }
                    previewModel = ProvenceManager.ModelBank.LoadModel(modelComponent.manualKey);
                    if(previewModel == null) return;
                    modelViewer.style.display = DisplayStyle.Flex;                    
                    IMGUIContainer modelViewerWrapper = new IMGUIContainer();
                    modelViewerWrapper.AddToClassList("model-preview-wrapper");
                    modelViewer.Add(modelViewerWrapper);
                    modelViewerWrapper.onGUIHandler = () =>{
                        DrawObjectPreview(modelViewerWrapper, ref modelViewerEditor, previewModel);
                    };
                } else modelViewer.style.display = DisplayStyle.None;
            }
        }

        #endregion       

        #region  ManualEditor manualMethods
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
        
        protected virtual void AddComponentToEntry<V>() where V : ProvenceComponent, new(){
            if(collection.ContainsKey(chosenKey) && !collection[chosenKey].components.ContainsKey(typeof(V))){
                V component = new V();
                collection[chosenKey].components[typeof(V)] = component;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected virtual void RemoveComponentFromEntry<V>() where V : ProvenceComponent{
            if(collection.ContainsKey(chosenKey) && collection[chosenKey].components.ContainsKey(typeof(V))){
                foreach(Type tag in collection[chosenKey].tags){
                    foreach(System.Type requiredType in ((MainframeTag)Activator.CreateInstance(tag)).requiredComponents) 
                        if(typeof(V) == requiredType) return;
                }
                collection[chosenKey].components.Remove(typeof(V));
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected virtual void AddTagToEntry(Type tag){
            if(chosenKey != null){
                collection[chosenKey].tags.Add(tag);
                foreach(System.Type requiredType in ((MainframeTag)Activator.CreateInstance(tag)).requiredComponents){
                    Helpers.InvokeGenericMethod(this, "AddComponentToEntry", requiredType);                    
                }
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected virtual void RemoveTagFromEntry(Type tag){
            if(chosenKey != null && tag != null){
                collection[chosenKey].tags.Remove(tag);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected virtual void RemoveTagCompleteFromEntry(Type tag){
            if(chosenKey != null && tag != null){
                collection[chosenKey].tags.Remove(tag);
                foreach(System.Type requiredType in ((MainframeTag)Activator.CreateInstance(tag)).requiredComponents){
                    Helpers.InvokeGenericMethod(this, "RemoveComponentFromEntry", requiredType);
                }
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected abstract void LoadCollection();

        protected void SaveManual(SceneSavedEvent args){
            collection.Save();
        }
        #endregion

    }

}