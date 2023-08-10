using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;

namespace ProvenceECS.Mainframe{

    public class ModelBankEditorUIDirectory : UIDirectory{
        public ModelBankEditorUIDirectory(){
            this.uxmlPath = provenceEditorRoot + @"/Core/ModelBank/ModelBankEditor.uxml";
            this.ussPaths = new string[]{
                provenceEditorRoot + @"/Core/ModelBank/ModelBankEditor.uss"
            };
        }
    }

    
    public class ModelBankEditor : ProvenceCollectionEditor<ModelBankEntry>{

        protected Texture addIcon;
        protected Texture searchIcon;
        protected Texture delIcon;
        protected Texture caretIcon;

        [MenuItem("ProvenceECS/Model Bank")]
        public static void ShowWindow(){
            ModelBankEditor window = GetWindow<ModelBankEditor>();
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Model Bank");
            this.uiDirectory = new ModelBankEditorUIDirectory();
            this.addIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/plus.png");
            this.searchIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/search.png");
            this.delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
            this.caretIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/caret-down.png");
        }

        protected override void DrawEntryEditor(){
            entryEditorScroller.Clear();    
            DrawEntryTitle();            
            DrawModelViewer();
            if(chosenKey == null) return;
            GameObject model = DrawObjectField();
            if(model != null){
                DrawDataControls();
                DrawAnchorControls(model);
                DrawAnimationControls(model);
            }
            DrawTagList();
        }

        protected GameObject DrawObjectField(){
            Div container;
            ListItem titleItem = DrawShelf("Model", out container);

            ListItem objectItem = new ListItem();
            objectItem.AddLabel("Resource:", true);
            ObjectField objectField = objectItem.AddObjectField(typeof(GameObject));

            GameObject gameObject = ((ModelBank)collection).LoadModel(chosenKey);
            if(gameObject != null){
                objectField.value = gameObject;
            }

            objectItem.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input == objectField){
                    string assetPath = AssetDatabase.GetAssetPath(objectField.value);
                    if(objectField.value == null){
                        ((ModelBank)collection).SetResourcePath(chosenKey, assetPath);
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
                        return;
                    }
                    if(assetPath.Contains(@"/Resources/")){
                        ((ModelBank)collection).SetResourcePath(chosenKey, assetPath);
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
                    }else{                            
                        if(objectField.value != null){ 
                            Debug.LogWarning("Not Resource");
                            objectField.value = null;
                        }
                    }
                }
            });

            container.Add(objectItem);
            entryEditorScroller.Add(titleItem,container);
            return gameObject;
        }

        protected void DrawDataControls(){
            Div container;
            ListItem titleItem = DrawShelf("Offsets", out container);

            FieldControl<Vector3> posOffsetControl = new FieldControl<Vector3>(collection[chosenKey].positionOffset,"position-offset-control","Position Offset");
            posOffsetControl.eventManager.AddListener<FieldControlUpdated<Vector3>>(e =>{
                collection[chosenKey].positionOffset = e.control.value;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });

            FieldControl<Vector3> rotOffsetControl = new FieldControl<Vector3>(collection[chosenKey].rotationOffset,"rotation-offset-control","Rotation Offset");
            rotOffsetControl.eventManager.AddListener<FieldControlUpdated<Vector3>>(e =>{
                collection[chosenKey].rotationOffset = e.control.value;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });

            FieldControl<Vector3> scaleOffsetControl = new FieldControl<Vector3>(collection[chosenKey].scaleOffset,"scale-offset-control","Scale");
            scaleOffsetControl.eventManager.AddListener<FieldControlUpdated<Vector3>>(e =>{
                collection[chosenKey].scaleOffset = e.control.value;
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            });

            container.Add(posOffsetControl,rotOffsetControl,scaleOffsetControl);
            entryEditorScroller.Add(titleItem,container);
        }

        protected void DrawAnchorControls(GameObject model){
            Div anchorContainer = new Div();
            
            ListItem titleItem = new ListItem();
            titleItem.AddToClassList("spacer","selectable","container-title");
            titleItem.AddImage(null);
            ListItemText title = titleItem.AddTitle("Anchors");
            ListItemImage addButton = titleItem.AddImage(addIcon);
            addButton.AddToClassList("hoverable","selectable");

            addButton.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                collection[chosenKey].anchors[""] = new ModelAnchorData();
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                DrawAnchorList(anchorContainer,model);
            });
            
            Div container = new Div();
            container.AddToClassList("category-container");
            
            title.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                else title.AddToClassList("second-alternate");
                container.Toggle();
            });

            ListItem searchBar = new ListItemSearchBar(anchorContainer);
            
            DrawAnchorList(anchorContainer,model);

            container.Add(searchBar,anchorContainer);
            entryEditorScroller.Add(titleItem,container);
        }

        protected void DrawAnchorList(Div container, GameObject model){
            container.Clear();
            bool alternate = false;
            foreach(KeyValuePair<string,ModelAnchorData> kvp in collection[chosenKey].anchors){
                ListItem anchorItem = new ListItem(alternate);
                anchorItem.AddToClassList("search-list-item");
                anchorItem.userData = kvp.Key;
                anchorItem.AddToClassList("anchor-list-item");

                KeySelectorElement keyInput = anchorItem.AddKeySelector("Anchor Name",kvp.Key,ModelAnchorKeys.keys);

                keyInput.eventManager.AddListener<MainframeKeySelection<string>>(e =>{
                    string newKey = e.value;
                    if(collection[chosenKey].anchors.ContainsKey(newKey)) return;
                    collection[chosenKey].anchors[newKey] = collection[chosenKey].anchors[kvp.Key];
                    collection[chosenKey].anchors.Remove(kvp.Key);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    DrawAnchorList(container,model);
                });

                HierarchySelectorElement hierarchyInput = anchorItem.AddHierarchySelector("Object", model, ModelBank.GetChildByHierarhcy(model, kvp.Value.hierarchy.ToArray()));
                hierarchyInput.eventManager.AddListener<MainframeKeySelection<int[]>>(e =>{
                    collection[chosenKey].anchors[kvp.Key] = new ModelAnchorData(hierarchyInput.objectDisplay.value.name, hierarchyInput.value);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    DrawAnchorList(container, model);
                });

                ListItemImage delButton = anchorItem.AddImage(delIcon);
                delButton.AddToClassList("selectable","hoverable","icon");

                delButton.eventManager.AddListener<MouseClickEvent>(e => {
                    collection[chosenKey].anchors.Remove(kvp.Key);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    DrawAnchorList(container,model);
                });                

                container.Add(anchorItem);
                alternate = !alternate;
            }
        }

        protected void DrawAnimationControls(GameObject model){
            Div animationDrawer = new Div();
            
            ListItem titleItem = new ListItem();
            titleItem.AddToClassList("spacer","selectable","container-title");
            titleItem.AddImage(null);
            ListItemText title = titleItem.AddTitle("Animation Keys");
            ListItemImage addButton = titleItem.AddImage(addIcon);
            addButton.AddToClassList("hoverable","selectable");

            addButton.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                collection[chosenKey].animationData[""] = new ModelAnimationData();
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                DrawAnimationDrawer(animationDrawer, model);
            });
            
            Div container = new Div();
            container.AddToClassList("category-container");
            
            title.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                else title.AddToClassList("second-alternate");
                container.Toggle();
            });

            ListItem searchBar = new ListItemSearchBar(animationDrawer);
            
            DrawAnimationDrawer(animationDrawer, model);

            container.Add(searchBar, animationDrawer);
            entryEditorScroller.Add(titleItem, container);
        }

        protected void DrawAnimationDrawer(Div container, GameObject model){
            container.Clear();
            bool alternate = false;
            Animator animComponent = model.GetComponent<Animator>();
            if(animComponent != null){            
                foreach(KeyValuePair<string, ModelAnimationData> kvp in collection[chosenKey].animationData){
                                        
                    ListItem item = new ListItem(alternate);
                    
                    KeySelectorElement keyButton = item.AddKeySelector("Animation Key", kvp.Key, Helpers.GetAllTypesFromBaseType(typeof(AnimationKey)).Select(key => key.Name).ToSet());
                    keyButton.eventManager.AddListener<MainframeKeySelection<string>>(e =>{
                        if(!collection[chosenKey].animationData.ContainsKey(e.value)){
                            string newKey = e.value;
                            collection[chosenKey].animationData[newKey] = collection[chosenKey].animationData[kvp.Key];
                            collection[chosenKey].animationData.Remove(kvp.Key);
                            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                            DrawAnimationDrawer(container, model);
                        }
                    });

                    item.AddLabel("State Name");
                    ListItemTextInput stateInput = item.AddTextField(kvp.Value.stateName);
                    stateInput.AddToClassList("animation-data-state-input");
                    stateInput.eventManager.AddListener<ListItemInputChange>(e => {
                        collection[chosenKey].animationData[kvp.Key].stateName = stateInput.text;
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    });

                    ListItemIntInput layerInput = item.AddIntField(collection[chosenKey].animationData[kvp.Key].layer);
                    layerInput.AddToClassList("animation-data-layer-input");
                    layerInput.eventManager.AddListener<ListItemInputChange>(e =>{
                        collection[chosenKey].animationData[kvp.Key].layer = layerInput.value;
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    });

                    ListItemImage delButton = item.AddImage(delIcon);
                    delButton.AddToClassList("selectable","hoverable","icon");

                    delButton.eventManager.AddListener<MouseClickEvent>(e => {
                        collection[chosenKey].animationData.Remove(kvp.Key);
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        DrawAnimationDrawer(container,model);
                    });

                    container.Add(item);
                    alternate = !alternate;
                }
            }
        }

        protected override void DrawModelViewer(){
            PageColumn modelViewer = root.Q<PageColumn>("model-viewer");
            modelViewer.Clear();
            if(chosenKey == null){
                modelViewer.style.display = DisplayStyle.None;               
                return;
            }            
            if(modelViewer != null){
                previewModel = ((ModelBank)collection).LoadModel(chosenKey);
                if(previewModel != null){
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

        protected override void DrawTagList(){
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
                                break;
                        }
                    });

                    tagListScroller.Add(tagElement);
                }
            }
        }

        protected override void AddTagToEntry(System.Type tag){
            if(chosenKey != null){
                collection[chosenKey].tags.Add(tag);
                eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(1));
            }
        }

        protected override void LoadCollection(){
            collection = ProvenceManager.ModelBank;
        }
        
    }

}