using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using ProvenceECS.Network;
using System.Threading.Tasks;

namespace ProvenceECS.Mainframe{

    public class InspectorUpdateEvent : MainframeUIArgs{
        public InspectorUpdateEvent(){}
    }

    public class ProvenceManagerEditorEntitySelection : MainframeUIArgs{

        public HashSet<Entity> entities;

        public ProvenceManagerEditorEntitySelection(HashSet<Entity> entities){
            this.entities = entities;
        }
    }

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

        public HashSet<Entity> currentlySelectedEntities;
        protected bool bubbleSelection = true;
        protected bool reselectRequired = false;

        protected EntityViewer entityViewer;
        protected ListItem bubbleSelectionButton;
        

        public ProvenceManagerEditor(){
            currentlySelectedEntities = new HashSet<Entity>();
        }

#region Window Methods

        [MenuItem("ProvenceECS/Manager &`")]
        public static void ShowWindow(){
            ProvenceManagerEditor window = GetWindow<ProvenceManagerEditor>();
        }

        public void OnInspectorUpdate(){
            if(reselectRequired && chosenKey != null){
                reselectRequired = false;
                new EditorSelectEntities(chosenKey, currentlySelectedEntities).Raise(chosenKey);
            }
            eventManager.Raise(new InspectorUpdateEvent());
            if(persistanceTimer < persistanceRate) persistanceTimer += Time.fixedDeltaTime;
            else{
                persistanceTimer = 0f;
                foreach(World world in ProvenceManager.Instance.worlds.Values){
                    world.eventManager.Raise(new EditorPersistanceUpdateEvent(world));
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

        void OnFocus(){
            entityViewer.windowFocused = true;
        }

        void OnLostFocus(){
            entityViewer.windowFocused = false;
        }

        public override void OnEnable(){            
            foreach(World world in ProvenceManager.Instance.worlds.Values){
                world.eventManager.Raise(new WorldSafetyDestroy(world,Time.time));
            }
            if(root != null) root.Clear();
            if(playModeState != PlayModeStateChange.ExitingEditMode){
                ProvenceSceneHook hook = FindObjectOfType<ProvenceSceneHook>();
                if(hook == null){
                    CreateNewManagerHook();
                    return;
                }
                ProvenceManager.Load(hook.id);
                SetMainWorldToScene();
            }
            base.OnEnable();
        }

        protected void CreateNewManagerHook(){
            GameObject managerObj = new GameObject("ProvenceECS Manager");
            ProvenceSceneHook sceneHook = managerObj.AddComponent<ProvenceSceneHook>();
            sceneHook.id = System.Guid.NewGuid().ToString();
            Debug.Log("New Provence Manager Created");
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            OnEnable();
        }

        protected void SetMainWorldToScene(){
            if(ProvenceManager.Instance.worlds.Count == 1 && !Application.isPlaying){
                ProvenceManager.Instance.worlds.ElementAt(0).Value.worldName = SceneManager.GetActiveScene().name;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            }
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

        protected override void SetReferences(){
            entityViewer = root.Q<EntityViewer>("entity-node-viewer");
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SceneLoadedEvent>(SceneLoaded);
            eventManager.AddListener<SelectKey<World>>(SelectKey);
            eventManager.AddListener<SceneSavedEvent>(SaveManager);
            RegisterEntityExplorerEventListeners();
            RegisterWorldEditorEventListeners();
        }

        protected void RegisterEntityExplorerEventListeners(){
            eventManager.AddListener<InspectorUpdateEvent>(entityViewer.InspectorUpdate);
            eventManager.AddListener<ProvenceManagerEditorEntitySelection>(entityViewer.SelectEntities);
            root.Q<ListItemImage>("world-table-refresh").eventManager.AddListener<MouseClickEvent>(e => {
                OnEnable();
            });
        }

        protected void RegisterWorldEditorEventListeners(){
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
            if(ProvenceManager.Instance.worlds.Count > 0)
                eventManager.Raise(new SelectKey<World>(ProvenceManager.Instance.worlds.First().Value));            
            InitializeBubbleButton();
            root.Q<ColumnScroller>("world-editor-system-scroller").Toggle();
        }

        protected void InitializeBubbleButton(){
            bubbleSelectionButton = new ListItem(){
                name = "bubble-selection"
            };

            ListItemText text = bubbleSelectionButton.AddTextDisplay("");
            if(bubbleSelection){
                text.text = "Select Only Entities: ON";
                bubbleSelectionButton.AddToClassList("selected");
            }else{
                text.text = "Select Only Entities: OFF";
                bubbleSelectionButton.RemoveFromClassList("selected");
            }

            bubbleSelectionButton.eventManager.AddListener<MouseClickEvent>(e=>{
                bubbleSelection = !bubbleSelection;
                if(bubbleSelection){
                    text.text = "Select Only Entities: ON";
                    bubbleSelectionButton.AddToClassList("selected");
                }else{
                    text.text = "Select Only Entities: OFF";
                    bubbleSelectionButton.RemoveFromClassList("selected");
                }
            });

            entityViewer.Add(bubbleSelectionButton);
        }

        protected override void SelectKey(SelectKey<World> args){
            currentlySelectedEntities.Clear();
            Selection.objects = new Object[0];
            chosenKey = args.key;
            if(chosenKey != null){
                entityViewer.SetWorld(chosenKey);
                EntityViewerSaveData data = Helpers.LoadFromSerializedFile<EntityViewerSaveData>(ProvenceCollection<AssetData>.dataPath + "/EntityViewer/" + chosenKey.id + ".meglo");
                if(data != null){
                    entityViewer.LoadSaveData(data);
                    bubbleSelection = data.bubbleSelection;
                }
                chosenKey.eventManager.Clear<EditorSelectEntities>();
                chosenKey.eventManager.AddListener<EditorSelectEntities>(SelectEntities);
                DrawSystemList();
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
                                eventManager.Raise(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                                DrawSystemList();
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
                                selected?.RemoveFromClassList("selected");
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


#endregion

#region Entity Methods

        void OnSelectionChange(){
            if(chosenKey != null){
                HashSet<Object> oldSelection = new HashSet<Object>(Selection.objects);
                HashSet<Object> newSelection = new HashSet<Object>();
                currentlySelectedEntities.Clear();
                foreach(Object obj in Selection.objects){
                    Object objToAddToSelection = obj;
                    EntityHandle entityHandle = chosenKey.LookUpEntity(obj.name);                    
                    if(entityHandle == null && bubbleSelection){
                        if(obj is GameObject go){
                            GameObject currentGo = go;
                            while(entityHandle == null){
                                if(currentGo.transform.parent == null) break;
                                currentGo = currentGo.transform.parent.gameObject;
                                entityHandle = chosenKey.LookUpEntity(currentGo.name);
                                if(entityHandle != null) objToAddToSelection = currentGo;
                            }
                        }
                    }
                    if(entityHandle != null) currentlySelectedEntities.Add(entityHandle.entity);
                    newSelection.Add(objToAddToSelection);
                }
                if(oldSelection.SetEquals(newSelection)){
                    eventManager.Raise(new ProvenceManagerEditorEntitySelection(currentlySelectedEntities));
                }else{
                    reselectRequired = true;                    
                }
            }
        }

        protected void SelectEntities(EditorSelectEntities args){
            HashSet<GameObject> objectsToSelect = new HashSet<GameObject>();
            foreach(Entity entity in args.entities){
                ComponentHandle<UnityGameObject> ugoHandle = args.world.GetComponent<UnityGameObject>(entity);
                if(ugoHandle != null){
                    objectsToSelect.Add(ugoHandle.component.gameObject);
                }
            }
            Selection.objects = objectsToSelect.ToArray();
        }

        public void DuplicateSelectedEntities(){
            if(chosenKey != null){
                eventManager.Raise(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                chosenKey.DuplicateEntities(currentlySelectedEntities.ToArray());
            }
        }

        public void DeleteSelectedEntities(){
            if(chosenKey != null){
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                chosenKey.RemoveEntities(currentlySelectedEntities.ToArray());
            }
        }

#endregion

#region System Methods

        protected void AddSystemButtonPressed(MouseClickEvent e){
            if(e.button != 0) return;
            List<System.Type> existingTypes = chosenKey.systemManager.GetCurrentSystemTypes();
            TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceSystem), false, chosenKey.systemManager.GetCurrentSystemTypes(), typeof(Entity));
            TypeSelector.Open(searchParameters, args =>{
                Helpers.InvokeGenericMethod(this,"AddSystem", args.value);
            });
        }

        protected void AddSystem<T>() where T : ProvenceSystem, new(){
            chosenKey.AddSystem<T>();
            eventManager.Raise(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            DrawSystemList();
        }

        protected void RemoveSystem<T>() where T : ProvenceSystem{
            chosenKey.RemoveSystem<T>();
            eventManager.Raise(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
            DrawSystemList();
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
                eventManager.Raise(new SetSceneDirtyEvent(EditorSceneManager.GetActiveScene()));
                DrawSystemList();
            });
            keySelector.eventManager.Raise(new SetSelectorParameters<HashSet<string>>(set));
        }

        protected void OpenSystemEditor<T>() where T : ProvenceSystem{
            if(chosenKey != null) SystemEditor.Show<T>(chosenKey.GetSystem<T>());
        }

#endregion

        //Manager Methods

        protected void SaveManager(SceneSavedEvent args){
            ProvenceManager.Instance.Save();
            EntityViewerSaveData data = entityViewer.GetSaveData();
            data.bubbleSelection = bubbleSelection;
            Helpers.SerializeAndSaveToFile(data, ProvenceCollection<AssetData>.dataPath + "/EntityViewer/", chosenKey.id, ".meglo");
        }
        
    }
}