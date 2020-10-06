using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ProvenceECS.Mainframe{

    public class EntityEditor : MainframeTableWindow<EntityHandle>{

        protected string windowID;

        protected ColumnScroller componentListScroller;
        protected ColumnScroller editorScroller;

        protected ComponentHandle<Name> chosenKeyNameHandle;
        protected GameObject chosenGameObject;
        protected List<ProvenceComponent> chosenComponents = new List<ProvenceComponent>();

        // Window Methods
  
        public static void Show(EntityHandle entityHandle){
            EntityEditor[] windows = Resources.FindObjectsOfTypeAll<EntityEditor>();
            for(int i = 0; i < windows.Length; i++){
                if(windows[i].CompareWindow(entityHandle)){
                    windows[i].Focus();
                    return;
                }
            }

            EntityEditor window = CreateWindow<EntityEditor>();
            window.titleContent = new GUIContent(entityHandle.entity.ToString());
            window.windowID = "entity-editor-" + System.Guid.NewGuid().ToString();
            window.eventManager.Raise<SelectKey<EntityHandle>>(new SelectKey<EntityHandle>(entityHandle));
        }

        public bool CompareWindow(EntityHandle entityHandle){
            return (entityHandle.entity == chosenKey.entity);
        }

        // Init Methods

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Provence Asset Manager");
            this.uiKey = "entity-editor";
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SelectKey<EntityHandle>>(SelectKey);
            eventManager.AddListener<SelectKey<ProvenceComponent>>(SelectKey);
            eventManager.AddListener<DrawColumnEventArgs<ProvenceComponent>>(DrawColumn);
            eventManager.AddListener<SceneSavedEvent>(SaveEntity);
            
            if(root == null) return;

            DropDownMenu archetypeMenu = root.Q<DropDownMenu>("archetype-menu");
            root.Q<ListItemText>("select-menu-button").eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
            });
            root.Q<ListItemText>("archetype-menu-button").eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                archetypeMenu.Show(root,e);
            });
            root.Q<ListItem>("add-component-button").eventManager.AddListener<MouseClickEvent>(AddComponentButtonClicked);

            root.Q<ListItemTextInput>("entity-name").eventManager.AddListener<ListItemInputChange>(e => {
                if(chosenKeyNameHandle != null){
                    string newName = ((ListItemTextInput)e.input).text;
                    if(chosenKeyNameHandle.component.name.Equals(newName)) return;
                    chosenKeyNameHandle.component.name = newName;
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                }
            });

            root.Q<ListItemText>("add-gameobject-button").eventManager.AddListener<MouseClickEvent>(AddGameObject);

            root.Q<ListItemText>("select-gameobject-button").eventManager.AddListener<MouseClickEvent>(e =>{
                if(chosenGameObject != null) Selection.objects = new Object[]{chosenGameObject};
            });

            root.Q<ListItemText>("remove-gameobject-button").eventManager.AddListener<MouseClickEvent>(RemoveGameObject);

        }

        protected override void InitializeWindow(){
            if(root == null) return;
            componentListScroller = root.Q<ColumnScroller>("component-list-scroller");
            editorScroller = root.Q<ColumnScroller>("editor-scroller");
        }

        protected override void BeforeAssemblyReload(){
            string dir = TableDirectory.GetSubKey("temp-editors",TableDirectoryKey.DIRECTORY);
            if(chosenKey != null) Helpers.SerializeAndSaveToFile<Entity>(chosenKey.entity,dir,windowID,".meglo");

        }

        protected override void AfterAssemblyReload(){
            if(windowID != null){
                string dir = TableDirectory.GetSubKey("temp-editors",TableDirectoryKey.DIRECTORY);
                string file = windowID + ".meglo";
                Entity entity = Helpers.LoadFromSerializedFile<Entity>(dir + file);
                if(entity != null) {
                    EntityHandle entityHandle = ProvenceManager.Instance.LookUpEntity(entity);
                    if(entityHandle != null) eventManager.Raise<SelectKey<EntityHandle>>(new SelectKey<EntityHandle>(entityHandle));
                }
                Helpers.Delay(() =>{
                    Helpers.DeleteFolderContents(dir);
                },100);
            }
        }

        //Data Methods

        protected override void SelectKey(SelectKey<EntityHandle> args){
            if(args.key == null) return;
            chosenKey = args.key;

            chosenKeyNameHandle = chosenKey.GetOrCreateComponent<Name>();
            chosenGameObject = chosenKey.GetGameObject();
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(0));
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(2));
        }

        protected void SelectKey(SelectKey<ProvenceComponent> args){
            if(chosenComponents.Contains(args.key)) chosenComponents.Remove(args.key);
            else chosenComponents.Add(args.key);
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(1));
        }

        protected void AddComponent<T>() where T : ProvenceComponent, new(){
            if(typeof(T) == typeof(Name)) return;
            if(chosenKey != null && chosenKey.GetComponent<T>() == null){
                chosenKey.AddComponent<T>();
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(0));
            }
        }

        protected void RemoveComponent<T>() where T : ProvenceComponent{
            if(typeof(T) == typeof(Name)) return;
            if(chosenKey != null) chosenKey.RemoveComponent<T>();
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(0));
        }

        protected void SaveEntity(SceneSavedEvent e){
            ProvenceManager.Instance.Save();
        }

        protected void AddGameObject(MouseClickEvent e){
            if(e.button != 0 || chosenGameObject != null || chosenKey == null) return;
            chosenGameObject = chosenKey.AddGameObject();
            Selection.objects = new Object[]{chosenGameObject};
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(2));
        }

        protected void RemoveGameObject(MouseClickEvent e){
            if(e.button != 0 || chosenGameObject == null || chosenKey == null) return;
            chosenKey.RemoveGameObject();
            chosenGameObject = null;
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(2));
        }

        // Interface Methods

        protected void DrawColumn(DrawColumnEventArgs<ProvenceComponent> args){
            switch(args.column){
                case 0:
                    DrawComponentList();
                    break;
                case 1:
                    DrawComponentEditor();
                    break;
                case 2:
                    DrawEntityData();
                    break;
            }
        }

        protected void DrawEntityData(){
            root.Q<ListItemTextInput>("entity-name").value = chosenKeyNameHandle.component.name;
            ListItemText addButton = root.Q<ListItemText>("add-gameobject-button");
            ListItemText selectButton = root.Q<ListItemText>("select-gameobject-button");
            ListItemText removeButton = root.Q<ListItemText>("remove-gameobject-button");
            if(chosenGameObject != null){
                addButton.style.display = DisplayStyle.None;
                selectButton.style.display = DisplayStyle.Flex;
                removeButton.style.display = DisplayStyle.Flex;
            }else{
                addButton.style.display = DisplayStyle.Flex;
                selectButton.style.display = DisplayStyle.None;
                removeButton.style.display = DisplayStyle.None;
            }
        }

        protected void DrawComponentList(){
            if(componentListScroller == null) return;
            componentListScroller.Clear();
            editorScroller.Clear();

            if(chosenKey != null){
                DropDownMenu contextMenu = root.Q<DropDownMenu>("component-list-context-menu");
                bool alternate = false;
                foreach(ComponentHandle<ProvenceComponent> handle in chosenKey.GetAllComponents()){
                    if(handle == null || handle.component == null || handle.component.GetType().IsDefined(typeof(DontDisplayInEditor),false)) continue;
                    DrawComponentListItem(handle, contextMenu, alternate);
                    alternate = !alternate;
                }
            }
        }

        protected void DrawComponentListItem(ComponentHandle<ProvenceComponent> handle, DropDownMenu contextMenu, bool alternate){
            ListItem item = new ListItem(alternate,true);
            string componentName = System.Text.RegularExpressions.Regex.Replace(handle.component.GetType().Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
            item.AddButton(componentName,false,false,true);
            
            item.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.element != item) return;
                switch(e.button){
                    case 0:
                        if(item.ClassListContains("selected")) item.RemoveFromClassList("selected");
                        else item.AddToClassList("selected");
                        eventManager.Raise<SelectKey<ProvenceComponent>>(new SelectKey<ProvenceComponent>(handle.component));
                        break;
                    case 1:
                        contextMenu.Show(root,e,true);
                        ListItemText removeButton = root.Q<ListItemText>("component-list-context-menu-remove-button");
                        removeButton.eventManager.ClearListeners();
                        removeButton.eventManager.AddListener<MouseClickEvent>(ev=>{
                            if(ev.button != 0) return;
                            if(chosenComponents.Contains(handle.component)) chosenComponents.Remove(handle.component);
                            Helpers.InvokeGenericMethod(this, "RemoveComponent", handle.component.GetType());
                            contextMenu.style.display = DisplayStyle.None;
                        });
                        break;
                }
            });

            componentListScroller.Add(item);
        }

        protected void AddComponentButtonClicked(MouseClickEvent e){
            if(e.button != 0) return;
            List<System.Type> existingTypes = new List<System.Type>();
            foreach(ComponentHandle<ProvenceComponent> handle in chosenKey.GetAllComponents())
                existingTypes.Add(handle.component.GetType());
            TypeSelector.TypeSelectorParameters searchParameters = new TypeSelector.TypeSelectorParameters(typeof(ProvenceComponent), false, existingTypes, typeof(Entity));
            TypeSelector.Open(searchParameters, args =>{
                Helpers.InvokeGenericMethod<EntityEditor>(this,"AddComponent", args.value);
            });
        }

        protected void DrawComponentEditor(){
            if(editorScroller == null) return;
            editorScroller.Clear();
            foreach(ProvenceComponent component in chosenComponents){
                Helpers.InvokeGenericMethod(this, "DrawStructureControl", component.GetType(), component);
            }
        }

        protected void DrawStructureControl<T>(T component) where T : ProvenceComponent{
            ListItem titleItem = new ListItem();
            titleItem.name = typeof(T).Name.ToLower() + "-control-title";
            titleItem.AddToClassList("spacer","structure-control-title");
            titleItem.AddTextDisplay(System.Text.RegularExpressions.Regex.Replace(component.GetType().Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"), true);
            StructureControl<T> control = new StructureControl<T>(ref component, component.GetType().Name, false, chosenKey.world, chosenKey.entity);
            control.eventManager.AddListener<StructureControlUpdated<T>>(e =>{
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });
            control.eventManager.AddListener<StructureControlRefreshRequest>(e =>{
                Helpers.Delay(()=>{
                    chosenComponents.Clear();
                    eventManager.Raise<DrawColumnEventArgs<ProvenceComponent>>(new DrawColumnEventArgs<ProvenceComponent>(0));
                },10);
            });
            editorScroller.Add(titleItem);
            editorScroller.Add(control);
        }

    }

}