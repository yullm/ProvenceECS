using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShortcutManagement;
using ProvenceECS.Network;

namespace ProvenceECS.Mainframe{

    public class ProvenceManagerUIDirectory : UIDirectory{
        public ProvenceManagerUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/_ProvenceManager/ProvenceManagerEditor.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/_ProvenceManager/ProvenceManagerEditor.uss"
            };
        }
    }

    public class ProvenceManagerEditor : MainframeTableWindow<World>{

        protected Texture worldTableItemIcon;
        protected Texture entityIcon;
        protected Texture systemIcon;
        protected Texture packageIcon;
        protected float persistanceRate = 0.2f;
        protected float persistanceTimer = 0f;
        protected float backupTimer = 0f;
        protected float backupTarget = 50f;

        [MenuItem("ProvenceECS/Manager &`")]
        public static void ShowWindow(){
            ProvenceManagerEditor window = GetWindow<ProvenceManagerEditor>();
        }

        public void OnInspectorUpdate(){
            if(persistanceTimer < persistanceRate) persistanceTimer += Time.fixedDeltaTime;
            else{
                persistanceTimer = 0f;
                foreach(World world in ProvenceManager.Instance.worlds.Values){
                    world.eventManager.Raise<EditorPersistanceUpdateEvent>(new EditorPersistanceUpdateEvent(world));
                }
            }
            if(backupTimer < backupTarget) backupTimer += Time.fixedDeltaTime;
            else{
                backupTimer = 0f;
                ProvenceManager.Instance.Backup();
            }
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Provence ECS");
            this.uiDirectory = new ProvenceManagerUIDirectory();
        }

        public override void OnEnable(){            
            foreach(World world in ProvenceManager.Instance.worlds.Values){
                world.eventManager.Raise(new WorldSafetyDestroy(world,Time.time));
            }
            if(playModeState == PlayModeStateChange.ExitingEditMode){
                ShortcutManager.instance.activeProfileId = "playmode";
            }
            if(playModeState == PlayModeStateChange.EnteredEditMode){
                ShortcutManager.instance.activeProfileId = "Default copy";
            }
            if(root != null) root.Clear();
            if(playModeState != PlayModeStateChange.ExitingEditMode){
                ProvenceSceneHook hook = FindObjectOfType<ProvenceSceneHook>();
                if(hook == null){
                    CreateNewManagerHook();
                    return;
                }
                ProvenceManager.Load(hook.id);
            }
            base.OnEnable();
        }

        protected void CreateNewManagerHook(){
            GameObject managerObj = new GameObject("ProvenceECS Manager");
            ProvenceSceneHook sceneHook = managerObj.AddComponent<ProvenceSceneHook>();
            sceneHook.id = System.Guid.NewGuid().ToString();
            Debug.Log("New Provence Manager Created");
            OnEnable();
        }

        protected override void EditorStateChange(PlayModeStateChange args){
            ProvenceSceneHook hook = FindObjectOfType<ProvenceSceneHook>();
            switch(args){
                case PlayModeStateChange.EnteredEditMode:
                    OnEnable();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    InitializeWindow();
                    break;
            }
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SceneLoadedEvent>(SceneLoaded);
            eventManager.AddListener<SelectKey<World>>(SelectKey);
            eventManager.AddListener<DrawColumnEventArgs<World>>(DrawColumn);
            eventManager.AddListener<SceneSavedEvent>(SaveManager);
            RegisterWorldTableEventListeners();
            RegisterWorldEditorEventListeners();
        }

        protected void SceneLoaded(SceneLoadedEvent args){
            ProvenceSceneHook hook = FindObjectOfType<ProvenceSceneHook>();
            if(hook == null){
                OnEnable();
                return;
            }else{
                if(hook.id != ProvenceManager.Instance.managerID){
                    OnEnable();
                    return;
                }
            }
        }

        protected void RegisterWorldTableEventListeners(){
            root.Q<ListItemImage>("world-table-refresh").eventManager.AddListener<MouseClickEvent>(e => {
                OnEnable();
            });

            root.Q<ListItemImage>("add-world-button").eventManager.AddListener<MouseClickEvent>(AddWorld);

            root.Q<ListItem>("actor-manual-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                EditorWindow.GetWindow<ActorManualEditor>();
            });

            root.Q<ListItem>("model-bank-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                EditorWindow.GetWindow<ModelBankEditor>();
            });
           
            root.Q<ListItem>("asset-manager-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                AssetManager.CreateAssetLibrary();
            });

            root.Q<ListItem>("packet-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                ProvenceNetwork.GeneratePacketDictionary();
                Helpers.SerializeAndSaveToFile(ProvenceNetwork.PacketDict.OrderBy(kvp => kvp.Key), ProvenceCollection<AssetData>.dataPath + "/Packets/", "provence-packets", ".meglo");
            });

            root.Q<ListItem>("system-package-manager-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                EditorWindow.GetWindow<SystemPackageManagerEditor>();
            }); 
        }

        protected void RegisterWorldEditorEventListeners(){
            ListItemTextInput deleteInput = root.Q<ListItemTextInput>("world-editor-delete-input");
            deleteInput.eventManager.AddListener<ListItemInputChange>(e =>{
                if(deleteInput.text.Equals("")) deleteInput.SetToPlaceholder();
            });
            deleteInput.eventManager.AddListener<ListItemInputCommit>(RemoveWorld);
            root.Q<ListItemImage>("world-editor-back-button").eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                deleteInput.SetToPlaceholder();
                eventManager.Raise<SelectKey<World>>(new SelectKey<World>(null));
                eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(0));
            });

            root.Q<ListItemTextInput>("world-editor-name-input").eventManager.AddListener<ListItemInputChange>(e => {
                if(!((ListItemTextInput)e.input).text.Equals(chosenKey.worldName)) 
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                if(chosenKey != null) chosenKey.worldName = ((ListItemTextInput)e.input).text;
            });

            root.Q<ListItemText>("world-editor-set-active-button").eventManager.AddListener<MouseClickEvent>(SetAsActiveWorld);
            
            root.Q<ListItemImage>("add-entity-button").eventManager.AddListener<MouseClickEvent>(CreateEntity);

            root.Q<ListItemImage>("world-editor-delete-button").eventManager.AddListener<MouseClickEvent>(RemoveWorld);

            root.Q<ListItemImage>("world-editor-entity-toggle").eventManager.AddListener<MouseClickEvent>(e => {
                root.Q<ColumnScroller>("world-editor-entity-scroller").Toggle();
            });

            root.Q<ListItemImage>("world-editor-system-toggle").eventManager.AddListener<MouseClickEvent>(e => {
                root.Q<ColumnScroller>("world-editor-system-scroller").Toggle();
            });
            
            root.Q<Div>("add-package-button").eventManager.AddListener<MouseClickEvent>(AddPackageButtonPressed);
            root.Q<Div>("add-system-button").eventManager.AddListener<MouseClickEvent>(AddSystemButtonPressed);
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            worldTableItemIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/globe-europe.png");
            entityIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/circle-notch.png");
            systemIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/leaf.png");
            packageIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/clone-open.png");
            eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(0));
        }

        protected override void DrawColumn(DrawColumnEventArgs<World> args){
            ClearPages();
            switch(args.column){
                case 0:
                    DrawWorldList();
                    break;
                case 1:
                    DrawWorldEditor();
                    break;
            }
        }

        protected override void SelectKey(SelectKey<World> args){
            chosenKey = args.key;
            if(chosenKey != null)
                eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
        }

        protected void DrawWorldList(){
            root.Q<MenuBar>("world-table-menu-bar").style.display = DisplayStyle.Flex;
            root.Q<PageColumn>("world-table").style.display = DisplayStyle.Flex;

            ColumnScroller scroller = root.Q<ColumnScroller>("world-table-scroller");
            scroller.Clear();
            bool alternate = false;

            foreach(World world in ProvenceManager.Instance.worlds.Values){

                ListItem item = new ListItem(alternate, true, true);
                item.name = world.id;
                item.AddIndent();
                ListItemImage icon = item.AddImage(worldTableItemIcon);
                icon.AddToClassList("icon");

                ListItemText nameDisplay = item.AddTextDisplay(world.worldName);
                nameDisplay.AddToClassList("selectable");
                nameDisplay.AddToClassList("world-table-item-name");
                
                item.eventManager.AddListener<MouseClickEvent>(e => {
                    if(e.button != 0) return;
                    if(item.ClassListContains("selected")){
                        eventManager.Raise<SelectKey<World>>(new SelectKey<World>(world));
                    }else{
                        VisualElement selected = root.Q<VisualElement>(null,"selected");
                        if(selected != null) selected.RemoveFromClassList("selected");
                        item.AddToClassList("selected");
                    }

                });

                scroller.Add(item);
                alternate = !alternate;
            }

        }

        protected void DrawWorldEditor(){
            root.Q<MenuBar>("world-editor-menu-bar").style.display = DisplayStyle.Flex;
            root.Q<PageColumn>("world-editor").style.display = DisplayStyle.Flex;
            root.Q<ListItemText>("world-editor-active-display").style.display = DisplayStyle.None;
            root.Q<ListItemText>("world-editor-set-active-button").style.display = DisplayStyle.None;

            /* if(ProvenceManager.Instance.activeWorld == chosenKey)
                root.Q<ListItemText>("world-editor-active-display").style.display = DisplayStyle.Flex;
            else
                root.Q<ListItemText>("world-editor-set-active-button").style.display = DisplayStyle.Flex; */

            ListItemTextInput nameInput = root.Q<ListItemTextInput>("world-editor-name-input");
            if(nameInput != null) nameInput.value = chosenKey.worldName;

            DrawEntityList();
            DrawSystemList();
            
        }

        protected void DrawEntityList(){
            ColumnScroller scroller = root.Q<ColumnScroller>("world-editor-entity-scroller");
            scroller.Clear();
            bool alternate = false;
            DropDownMenu contextMenu = root.Q<DropDownMenu>("entity-list-context-menu");
            foreach(EntityHandle entityHandle in chosenKey.LookUpAllEntities()){

                ListItem item = new ListItem(alternate,true,true);
                item.AddIndent();
                item.AddImage(entityIcon).AddToClassList("icon");

                ComponentHandle<Name> nameHandle = entityHandle.GetComponent<Name>();
                if(nameHandle != null) item.AddTextDisplay(nameHandle.component.name);
                else item.AddTextDisplay(entityHandle.entity.ToString());

                item.eventManager.AddListener<MouseClickEvent>(e => {
                    switch(e.button){
                        case 0:
                            ComponentHandle<UnityGameObject> objectHandle = entityHandle.GetComponent<UnityGameObject>();
                            GameObject entityGO = objectHandle != null ? objectHandle.component.gameObject : null;
                            if(entityGO != null){
                                Selection.objects = new Object[]{entityGO};
                            }
                            if(item.ClassListContains("selected")){
                                EntityEditor.Show(entityHandle);
                            }else{
                                ListItem selected = scroller.Q<ListItem>(null,"selected");
                                if(selected != null) selected.RemoveFromClassList("selected");
                                item.AddToClassList("selected");
                            }
                            break;
                        case 1:
                            contextMenu.Show(root,e,true);
                            ListItemText duplicateButton = root.Q<ListItemText>("entity-list-context-menu-duplicate-button");
                            duplicateButton.eventManager.ClearListeners();
                            duplicateButton.eventManager.AddListener<MouseClickEvent>(ev =>{
                                if(ev.button != 0) return;
                                DuplicateEntity(entityHandle);
                                contextMenu.style.display = DisplayStyle.None;
                            });

                            ListItemText removeButton = root.Q<ListItemText>("entity-list-context-menu-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                if(ev.button != 0) return;
                                RemoveEntity(entityHandle);
                                contextMenu.style.display = DisplayStyle.None;
                            });
                            break;
                    }
                });

                scroller.Add(item);
                alternate = !alternate;
            }
        }

        protected void DrawSystemList(){
            ColumnScroller scroller = root.Q<ColumnScroller>("world-editor-system-scroller");
            scroller.Clear();
            bool alternate = false;
            DropDownMenu contextMenu = root.Q<DropDownMenu>("system-list-context-menu");

            foreach(string package in chosenKey.systemManager.systemPackages){
                ListItem item = new ListItem(alternate,true,true);
                item.AddIndent();
                item.AddImage(packageIcon).AddToClassList("icon");
                item.AddTextDisplay(package);
                scroller.Add(item);

                item.eventManager.AddListener<MouseClickEvent>(e => {
                    switch(e.button){
                        case 0:                            
                            ListItem selected = scroller.Q<ListItem>(null,"selected");
                            if(selected != null) selected.RemoveFromClassList("selected");
                            item.AddToClassList("selected");                            
                            break;
                        case 1:
                            contextMenu.Show(root,e,true);
                            ListItemText removeButton = root.Q<ListItemText>("system-list-context-menu-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                if(ev.button != 0) return;
                                chosenKey.systemManager.systemPackages.Remove(package);
                                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                                eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
                            });
                            break;
                    }
                });
            
                alternate = !alternate;
            }

            foreach(System.Type systemType in chosenKey.systemManager.GetCurrentSystemTypes()){
                ListItem item = new ListItem(alternate,true,true);
                item.AddIndent();
                item.AddImage(systemIcon).AddToClassList("icon");
                item.AddTextDisplay(Regex.Replace(systemType.Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));

                item.eventManager.AddListener<MouseClickEvent>(e => {
                    switch(e.button){
                        case 0:
                            if(item.ClassListContains("selected")){
                                Helpers.InvokeGenericMethod(this, "OpenSystemEditor", systemType);
                            }else{
                                ListItem selected = scroller.Q<ListItem>(null,"selected");
                                if(selected != null) selected.RemoveFromClassList("selected");
                                item.AddToClassList("selected");
                            }
                            break;
                        case 1:
                            contextMenu.Show(root,e,true);
                            ListItemText removeButton = root.Q<ListItemText>("system-list-context-menu-remove-button");
                            removeButton.eventManager.ClearListeners();
                            removeButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                if(ev.button != 0) return;
                                Helpers.InvokeGenericMethod(this, "RemoveSystem" ,systemType);
                                contextMenu.style.display = DisplayStyle.None;
                            });
                            break;
                    }
                });

                scroller.Add(item);
                alternate = !alternate;
            }
        }

        protected void ClearPages(){
            root.Q<MenuBar>("world-table-menu-bar").style.display = DisplayStyle.None;
            root.Q<PageColumn>("world-table").style.display = DisplayStyle.None;

            root.Q<MenuBar>("world-editor-menu-bar").style.display = DisplayStyle.None;
            root.Q<PageColumn>("world-editor").style.display = DisplayStyle.None;
        }

        //World Table Methods

        protected void AddWorld(MouseClickEvent e){
            if(e.button != 0) return;
            // ProvenceManager.Instance.AddWorld(new World());
            // eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            // eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(0));
        }

        protected void RemoveWorld(ListItemInputCommit e){
            RemoveWorld();
        }

        protected void RemoveWorld(MouseClickEvent e){
            if(e.button != 0) return;
            RemoveWorld();
        }

        protected void RemoveWorld(){
            ListItemTextInput deleteInput = root.Q<ListItemTextInput>("world-editor-delete-input");
            if(deleteInput.text.Equals("DELETE")) {
                deleteInput.SetToPlaceholder();            
                ProvenceManager.Instance.RemoveWorld(chosenKey);
                eventManager.Raise<SelectKey<World>>(new SelectKey<World>(null));
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(0));
            }
        }


        //World Editor Methods

        protected void SetAsActiveWorld(MouseClickEvent e){
            if(e.button != 0) return;
            //ProvenceManager.Instance.activeWorld = chosenKey;
            root.Q<ListItemText>("world-editor-set-active-button").style.display = DisplayStyle.None;
            root.Q<ListItemText>("world-editor-active-display").style.display = DisplayStyle.Flex;
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
        }

        protected void CreateEntity(MouseClickEvent e){
            if(e.button != 0) return;
            chosenKey.CreateEntity();
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
        }

        protected void RemoveEntity(EntityHandle entityHandle){
            chosenKey.RemoveEntity(entityHandle.entity);
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            eventManager.Raise<SelectKey<World>>(new SelectKey<World>(chosenKey));
        }

        protected void DuplicateEntity(EntityHandle entityHandle){
            entityHandle.Duplicate();
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            eventManager.Raise<SelectKey<World>>(new SelectKey<World>(chosenKey));
        }

        protected void AddSystemButtonPressed(MouseClickEvent e){
            if(e.button != 0) return;
            List<System.Type> existingTypes = chosenKey.systemManager.GetCurrentSystemTypes();
            TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceSystem), false, chosenKey.systemManager.GetCurrentSystemTypes(), typeof(Entity));
            TypeSelector.Open(searchParameters, args =>{
                Helpers.InvokeGenericMethod<ProvenceManagerEditor>(this,"AddSystem", args.value);
            });
        }

        protected void AddSystem<T>() where T : ProvenceSystem, new(){
            chosenKey.AddSystem<T>();
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
        }

        protected void RemoveSystem<T>() where T : ProvenceSystem{
            chosenKey.RemoveSystem<T>();
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
        }

        protected void AddPackageButtonPressed(MouseClickEvent e){
            if(e.button != 0) return;
            HashSet<string> set = new HashSet<string>();
            foreach(string key in ProvenceManager.Collections<SystemPackage>().Keys){
                if(!chosenKey.systemManager.systemPackages.Contains(key))
                    set.Add(key);
            }
            KeySelector keySelector = KeySelector.Open();
            keySelector.eventManager.AddListener<MainframeKeySelection<string>>(ev =>{
                chosenKey.systemManager.systemPackages.Add(ev.value);
                chosenKey.systemManager.systemPackages = new HashSet<string>(chosenKey.systemManager.systemPackages.OrderBy(p => p));
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<World>>(new DrawColumnEventArgs<World>(1));
            });
            keySelector.eventManager.Raise<SetSelectorParameters<HashSet<string>>>(new SetSelectorParameters<HashSet<string>>(set));
        }

        protected void OpenSystemEditor<T>() where T : ProvenceSystem{
            if(chosenKey != null) SystemEditor.Show<T>(chosenKey.GetSystem<T>());
        }

        //Manager Methods

        protected override void AfterAssemblyReload(){
            //OnEnable();
        }

        protected void SaveManager(SceneSavedEvent args){
            eventManager.Raise<SelectKey<World>>(new SelectKey<World>(chosenKey));
            ProvenceManager.Instance.Save();
        }
        
    }
}