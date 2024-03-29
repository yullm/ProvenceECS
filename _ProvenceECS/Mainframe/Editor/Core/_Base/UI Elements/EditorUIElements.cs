﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{
    
    //Elements

    public partial class ListItem : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ListItem> {} 

        public ListItem(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("list-item");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }
      
        public ListItem(bool alternate = false, bool selectable = false, bool hoverable = false){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("list-item");
            if(alternate) this.AddToClassList("alternate");
            if(selectable) this.AddToClassList("selectable");
            if(hoverable) this.AddToClassList("hoverable");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

        public ListItemText AddLabel(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemText el = new ListItemText(text,alternate,secondAlternate,thirdAlternate);
            el.AddToClassList("list-item-label");
            this.Add(el);
            return el;
        }

        public ListItemText AddTextDisplay(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemText el = new ListItemText(text,alternate,secondAlternate,thirdAlternate);
            el.AddToClassList("list-item-text-display");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            this.Add(el);
            return el;
        }

        public ListItemText AddTitle(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemText el = new ListItemText(text,alternate,secondAlternate,thirdAlternate);
            el.AddToClassList("list-item-title");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            this.Add(el);
            return el;
        }

        public ListItemImage AddImage(Texture texture, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemImage el = new ListItemImage(texture,alternate,secondAlternate,thirdAlternate);
            this.Add(el);
            return el;
        }

        public Div AddIndent(bool alternate = false){
            Div el = new Div();
            el.AddToClassList("list-item-indent");
            if(alternate) el.AddToClassList("alternate");
            this.Add(el);
            return el;
        }

        public Div AddDiv(bool alternate = false){
            Div el = new Div();
            if(alternate) el.AddToClassList("alternate");
            this.Add(el);
            return el;
        }


        //Inputs

        public ListItemText AddButton(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemText el = new ListItemText(text, alternate, secondAlternate, thirdAlternate);
            el.AddToClassList("list-item-button");
            el.AddToClassList("selectable");
            this.Add(el);
            return el;
        }

        public EnumField AddBoolean(bool value = false, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            EnumField el = new EnumField(value ? BooleanEnum.TRUE : BooleanEnum.FALSE);
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-enum-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public EnumField AddEnumField(Enum value, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            EnumField el = new EnumField(value);
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-enum-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public ListItemIntInput AddIntField(int value = 0, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemIntInput el = new ListItemIntInput();
            el.value = value;
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public FloatField AddFloatField(float value = 0f, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            FloatField el = new FloatField();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-float-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public ListItemTextInput AddTextField(string text = "", bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ListItemTextInput el = new ListItemTextInput(text);
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.eventManager.AddListener<ListItemInputChange>(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public Vector3Field AddVector3Field(Vector3 value = new Vector3(), bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            Vector3Field el = new Vector3Field();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-vector-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public Vector4Field AddVector4Field(Vector4 value = new Vector4(), bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            Vector4Field el = new Vector4Field();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-vector-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public Vector2Field AddVector2Field(Vector2 value = new Vector2(), bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            Vector2Field el = new Vector2Field();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-vector-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public ObjectField AddObjectField(Type objectType, bool allowScene = false, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ObjectField el = new ObjectField();
            el.allowSceneObjects = allowScene;
            el.objectType = objectType;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-object-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public ColorField AddColorField(Color value = new Color(), bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            ColorField el = new ColorField();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-color-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
            el.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public ListItemText AddEntitySelector(World world, Entity value = null){
            
            ListItemText el = new ListItemText();
            el.AddToClassList("list-item-text-display");
            el.AddToClassList("hoverable");
            el.AddToClassList("selectable");
            if(value != null) el.text = value.ToString();

            EntityHandle handle = world.LookUpEntity((Entity)el.text);
            if(handle == null){
                el.text = "";
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            }
            el.eventManager.AddListener<MouseClickEvent>(e =>{
                handle = world.LookUpEntity((Entity)el.text);
                if(handle != null){
                    ComponentHandle<UnityGameObject> objectHandle = handle.GetComponent<UnityGameObject>();
                    GameObject go = objectHandle != null ? objectHandle.component.gameObject : null;
                    if(go != null) Selection.objects = new UnityEngine.Object[]{go};
                }
            });

            ListItemImage button = new ListItemImage("Assets/Icons/caret-down.png");
            button.AddToClassList("hoverable");
            button.AddToClassList("selectable");
            button.eventManager.AddListener<MouseClickEvent>(e =>{
                EntitySelector.Open(world, args =>{
                    el.text = args.value.ToString();
                    eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
                });
            });

            ListItemImage clearButton = new ListItemImage("Assets/Icons/times.png");
            clearButton.AddToClassList("hoverable");
            clearButton.AddToClassList("selectable");
            clearButton.eventManager.AddListener<MouseClickEvent>(e => {
                if(el.text.Equals("")) return;
                el.text = "";
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });

            this.Add(el);
            this.Add(button);
            this.Add(clearButton);
            return el;
        }

        public KeySelectorElement AddKeySelector(string labelText, string value, HashSet<string> keys, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            KeySelectorElement keySelector = new KeySelectorElement(labelText,value,keys);
            keySelector.eventManager.AddListener<MainframeKeySelection<string>>(e =>{
                this.eventManager.Raise<MainframeKeySelection<string>>(e);
            });
            AddAlternates(keySelector, alternate, secondAlternate, thirdAlternate);
            this.Add(keySelector);
            return keySelector;
        }

        public HierarchySelectorElement AddHierarchySelector(string labelText, GameObject root, GameObject objectValue = null, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            HierarchySelectorElement hierarchySelector = new HierarchySelectorElement(labelText, root, objectValue);
            hierarchySelector.eventManager.AddListener<MainframeKeySelection<int[]>>(e =>{
                this.eventManager.Raise<MainframeKeySelection<int[]>>(e);
            });
            AddAlternates(hierarchySelector, alternate, secondAlternate, thirdAlternate);
            this.Add(hierarchySelector);
            return hierarchySelector;
        }

        public static void AddAlternates(VisualElement el, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            if(alternate) el.AddToClassList("alternate");
            if(secondAlternate) el.AddToClassList("second-alternate");
            if(thirdAlternate) el.AddToClassList("third-alternate");
        }
        
        public void MakeShelf(string titleText, out Div container){
            this.AddToClassList("spacer","selectable","container-title");
            ListItemText title = AddTitle(titleText);
            container = new Div();
            container.AddToClassList("category-container");
            
            Div containerRef = container;
            eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(title.ClassListContains("second-alternate")) title.RemoveFromClassList("second-alternate");
                else title.AddToClassList("second-alternate");
                containerRef.Toggle();
            });
        }

    }

    public class ListItemText : TextElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ListItemText, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "text" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ListItemText)ve).text = textAttribute.GetValueFromBag(bag, cc);
            }
        }

        public ListItemText(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
            this.RegisterCallback<FocusOutEvent>(e => {
                eventManager.Raise<LoseFocus>(new LoseFocus(this));
            });
        }

        public ListItemText(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            this.text = text;
            this.eventManager = new EventManager<MainframeUIArgs>();

            ListItem.AddAlternates(this,alternate,secondAlternate,thirdAlternate);  

            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }
       
    }

    public class ListItemIntInput : IntegerField{
        public EventManager<MainframeUIArgs> eventManager;
        public new class UxmlFactory : UxmlFactory<ListItemIntInput, UxmlTraits> {}

        public ListItemIntInput(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            AddToClassList("list-item-input");
            AddToClassList("list-item-int-input");
            InitializeEvents();
        }

        public ListItemIntInput(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false) : this(){
            this.text = text;
            ListItem.AddAlternates(this,alternate,secondAlternate,thirdAlternate);
        }

        private void InitializeEvents(){
            this.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(this));
            });
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
            this.RegisterCallback<FocusOutEvent>(e => {
                eventManager.Raise<LoseFocus>(new LoseFocus(this));
            });
            this.RegisterCallback<KeyUpEvent>(e =>{
                if(e.keyCode == KeyCode.Escape) eventManager.Raise<ListItemInputCancel>(new ListItemInputCancel(this));
                if(e.keyCode == KeyCode.Return) eventManager.Raise<ListItemInputCommit>(new ListItemInputCommit(this));
                if(e.keyCode == KeyCode.KeypadEnter) eventManager.Raise<ListItemInputCommit>(new ListItemInputCommit(this));
            });
        }
    }

    public class ListItemTextInput : TextField{

        public EventManager<MainframeUIArgs> eventManager;

        public string placeholder;

        public new class UxmlFactory : UxmlFactory<ListItemTextInput, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "text" };
            UxmlStringAttributeDescription placeholderAttribute = new UxmlStringAttributeDescription { name = "placeholder" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ListItemTextInput)ve).text = textAttribute.GetValueFromBag(bag, cc);
                string placeholderText = placeholderAttribute.GetValueFromBag(bag, cc);
                if(!placeholderText.Equals("") && placeholderText != null){
                    ((ListItemTextInput)ve).placeholder = placeholderText;
                    ((ListItemTextInput)ve).text = ((ListItemTextInput)ve).placeholder;
                }
            }
        }

        public ListItemTextInput(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            AddToClassList("list-item-input");
            AddToClassList("list-item-text-input");
            InitializeEvents();
        }

        public ListItemTextInput(string text, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            this.text = text;
            this.eventManager = new EventManager<MainframeUIArgs>();
            AddToClassList("list-item-input");
            AddToClassList("list-item-text-input");
            ListItem.AddAlternates(this,alternate,secondAlternate,thirdAlternate);  
            InitializeEvents();
        }

        private void InitializeEvents(){
            this.RegisterValueChangedCallback(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(this));
            });
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
            this.RegisterCallback<FocusOutEvent>(e => {
                eventManager.Raise<LoseFocus>(new LoseFocus(this));
            });
            this.RegisterCallback<KeyUpEvent>(e =>{
                if(e.keyCode == KeyCode.Escape) eventManager.Raise<ListItemInputCancel>(new ListItemInputCancel(this));
                if(e.keyCode == KeyCode.Return) eventManager.Raise<ListItemInputCommit>(new ListItemInputCommit(this));
                if(e.keyCode == KeyCode.KeypadEnter) eventManager.Raise<ListItemInputCommit>(new ListItemInputCommit(this));
            });
        }

        public void SetToPlaceholder(){
            if(placeholder != null) this.text = placeholder;
        }
       
    }

    public class ListItemImage : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public Texture image;

        public new class UxmlFactory : UxmlFactory<ListItemImage, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "src" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return null;
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ListItemImage)ve).SetImage(AssetDatabase.LoadAssetAtPath<Texture>(textAttribute.GetValueFromBag(bag, cc)));
            }
        }

        public ListItemImage(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

        public ListItemImage(string path, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.SetImage(AssetDatabase.LoadAssetAtPath<Texture>(path));
            ListItem.AddAlternates(this,alternate,secondAlternate,thirdAlternate);
            InitializeEvents();
        }

        public ListItemImage(Texture texture, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.SetImage(texture);
            ListItem.AddAlternates(this,alternate,secondAlternate,thirdAlternate);
            InitializeEvents();
        }

        protected void InitializeEvents(){
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
            this.RegisterCallback<FocusOutEvent>(e => {
                eventManager.Raise<LoseFocus>(new LoseFocus(this));
            });
        }

        public void SetImage(Texture texture){
            this.image = texture;
            this.style.backgroundImage = (StyleBackground)image;
        }

    }

    public class ListItemSearchBar : ListItem{

        public VisualElement container;
        public ListItemTextInput searchInput;
        protected Texture searchIcon;
        protected Texture delIcon;

        public new class UxmlFactory : UxmlFactory<ListItemSearchBar, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription containerAttribute = new UxmlStringAttributeDescription { name = "container" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return null;
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ListItemSearchBar)ve).container = ve.Q<VisualElement>(containerAttribute.GetValueFromBag(bag,cc));
            }
        }

        public ListItemSearchBar(){
            this.container = null;
            this.searchIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/search.png");
            this.delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
            InitializeSearchBar();
        }

        public ListItemSearchBar(VisualElement container){
            this.container = container;
            this.searchIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/search.png");
            this.delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
            InitializeSearchBar();
        }

        protected void InitializeSearchBar(){
            this.AddToClassList("spacer","alternate");
            this.searchInput = this.AddTextField();
            this.AddImage(searchIcon).AddToClassList("icon","selectable","hoverable");
            ListItemImage searchClearButton = this.AddImage(delIcon);
            searchClearButton.AddToClassList("icon","selectable","hoverable");

            searchInput.eventManager.AddListener<ListItemInputChange>(e =>{
                string[] queries = searchInput.value.Replace(" ","").Split('/');
                container.Query("","search-list-item").ForEach(item => {
                    item.style.display = DisplayStyle.Flex;
                });

                if(!searchInput.value.Equals("")){
                    container.Query("","search-list-item").ForEach(item => {
                        string itemKey = ((string)item.userData);
                        if(itemKey == null || !ValueContainsQueries(itemKey, queries))
                            item.style.display = DisplayStyle.None;
                    });
                }
            });
            searchInput.eventManager.AddListener<ListItemInputCancel>(e => {
                searchInput.value = "";
            });
            searchClearButton.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button == 0) searchInput.eventManager.Raise<ListItemInputCancel>(new ListItemInputCancel(searchInput));
            });
        }

        protected bool ValueContainsQueries(string value, string[] queries){
            foreach(string query in queries){
                if(query.Length == 0) return false;
                if(value.ToLower().Contains(query.ToLower())) return true;
            }
            return false;
        }

    }

    public class KeySelectorElement : Div{
        public string value;
        public HashSet<string> keys;
        public ListItemText label;
        public ListItemText keyDisplay;
        public ListItemImage button;
        protected Texture caretIcon;

        public KeySelectorElement(string labelText, string value, HashSet<string> keys){
            this.value = value;
            this.keys = keys;
            this.caretIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/caret-down.png");
            InitializeElement(labelText,value);
        }

        protected void InitializeElement(string labelText, string value){
            label = new ListItemText(labelText);
            label.AddToClassList("list-item-label","alternate");
            
            keyDisplay = new ListItemText(value);
            keyDisplay.AddToClassList("list-item-text-display");

            button = new ListItemImage(caretIcon);
            button.AddToClassList("selectable","hoverable");

            button.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                KeySelector keySelector = KeySelector.Open();
                keySelector.eventManager.Raise<SetSelectorParameters<HashSet<string>>>(new SetSelectorParameters<HashSet<string>>(keys));
                keySelector.eventManager.AddListener<MainframeKeySelection<string>>(ev =>{
                    keyDisplay.text = ev.value;
                    value = ev.value;
                    this.eventManager.Raise<MainframeKeySelection<string>>(ev);
                });
            });

            this.Add(label,keyDisplay,button);
        }

    }

    public class HierarchySelectorElement : Div{
        public GameObject root;
        public GameObject objectValue;
        public int[] value;

        public ListItemText label;
        public ObjectField objectDisplay;
        public ListItemImage button;
        protected Texture caretIcon;

        public HierarchySelectorElement(string labelText, GameObject root, GameObject objectValue = null){
            this.root = root;
            this.objectValue = objectValue;
            this.value = null;
            this.caretIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/caret-down.png");
            InitializeElement(labelText);
        }

        protected void InitializeElement(string labelText){
            label = new ListItemText(labelText);
            label.AddToClassList("list-item-label");

            objectDisplay = new ObjectField{
                allowSceneObjects = false,
                objectType = typeof(GameObject),
                value = objectValue
            };
            objectDisplay.AddToClassList("list-item-input","list-item-object-input","second-alternate");

            button = new ListItemImage(caretIcon);
            button.AddToClassList("selectable","hoverable");

            button.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                HierarchySelector hierarchySelector = HierarchySelector.Open();
                hierarchySelector.eventManager.Raise(new SetSelectorParameters<GameObject>(root));
                hierarchySelector.eventManager.AddListener<MainframeKeySelection<int[]>>(ev =>{
                    this.value = ev.value;
                    this.objectDisplay.value = HierarchySelector.GetChildByHierarhcy(root, ev.value);
                    this.eventManager.Raise<MainframeKeySelection<int[]>>(ev);
                });
            });

            this.Add(label, objectDisplay, button);
        }
    }

    public class EntityViewerSaveData : NodeViewerSaveData<Entity>{
        public bool bubbleSelection;

        public EntityViewerSaveData(){
            this.bubbleSelection = true;
        }

        public EntityViewerSaveData(Node<Entity>[] nodeCache): base(nodeCache){
            this.bubbleSelection = true;
        }
    }

    public class EntityViewer : NodeViewer<Entity>{

        protected World world;
        protected HashSet<Node<Entity>> draggingNodes;

        protected Texture entityIcon;
        
        protected Div selectionSquare;
        protected ProvenceText worldLabel;

        protected DropDownMenu worldContextMenu;
        protected ListItemText addEntityButton;
        
        protected DropDownMenu entityContextMenu;
        protected ListItemText openButton;
        protected ListItemText duplicateButton;
        protected ListItemText deleteButton;

        protected bool hasDraggedNodes;
        protected Vector2 lastNodePosition;
        protected Vector2 boxSelectionStartingPosition;

        protected bool checkOverlap = false;
        protected float nameUpdateCounter = 0;
        protected float nameUpdateGoal = 1f;

        //protected Div testDiv;

        public new class UxmlFactory : UxmlFactory<EntityViewer> {}

        public EntityViewer() : base(){
            this.world = null;
            this.draggingNodes = new HashSet<Node<Entity>>();
            this.hasDraggedNodes = false;
            this.lastMousePosition = new Vector2();
            this.boxSelectionStartingPosition = new Vector2();
        }

        public EntityViewer(World world): this(){
            this.world = world;
        }

        protected override void InitializeElement(){
            entityIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/circle-notch.png");
            base.InitializeElement();

            worldLabel = new ProvenceText();
            worldLabel.AddToClassList("entity-viewer-world-name");

            selectionSquare = new Div();
            selectionSquare.AddToClassList("entity-viewer-selector");
            selectionSquare.style.display = DisplayStyle.None;
            
            this.Add(selectionSquare, worldLabel);

            CreateWorldContextMenu();
            CreateEntityContextMenu();
            
            anchor.style.scale = new StyleScale(new Scale(new(1,1,1)));

            /* testDiv = new Div();
            testDiv.AddToClassList("entity-viewer-test");
            anchor.Add(testDiv); */
        }

        protected void CreateWorldContextMenu(){
            worldContextMenu = new DropDownMenu();

            ListItem entityItem = new ListItem();
            addEntityButton = entityItem.AddButton("Add Entity",false,false,true);

            worldContextMenu.Add(entityItem);
            this.Add(worldContextMenu);
        }

        protected void CreateEntityContextMenu(){
            entityContextMenu = new DropDownMenu();

            ListItem openItem = new ListItem();
            openButton = openItem.AddButton("Open",false,false,true);

            ListItem dupItem = new ListItem();
            duplicateButton = dupItem.AddButton("Duplicate Selection",false,false,true);

            ListItem delItem = new ListItem();
            deleteButton = delItem.AddButton("Delete Selection",false,false,true);

            entityContextMenu.Add(openItem, dupItem, delItem);
            this.Add(entityContextMenu);
        }

        protected override void RegisterEventListeners(){
            this.RegisterCallback<MouseLeaveEvent>(e => {
                StopDraggingNodes(e.mousePosition);
                StopBoxSelection();
            });
            this.RegisterCallback<MouseDownEvent>(e => {
                if(e.button == 0 && e.target == this) StartBoxSelection(e);
            });
            this.RegisterCallback<MouseUpEvent>(e =>{
                if(e.button == 0){
                    //if(!e.shiftKey && e.target == this) Selection.objects = new UnityEngine.Object[0];
                    BoxSelect(e);
                    StopBoxSelection();  
                }
                if(e.button == 1){
                    if(e.target == this && !hasDraggedAnchor){
                        AnchorItemPositionFromMouse(e, out Vector2 position);
                        lastNodePosition = position;
                        Vector2 positionOffset = e.mousePosition - new Vector2(75,65);
                        worldContextMenu.Show(this, positionOffset);
                        addEntityButton.eventManager.ClearListeners();
                        addEntityButton.eventManager.AddListener<MouseClickEvent>(ev => {
                            if(ev.button != 0) return;                            
                            world.CreateEntity();
                            worldContextMenu.style.display = DisplayStyle.None;
                        });
                    }
                }
            });
            this.RegisterCallback<KeyDownEvent>(e =>{
                if(e.target != this) return;
                if(e.keyCode == KeyCode.T){                    
                    e.StopPropagation();
                    if(e.ctrlKey) ArrangeByWorldSpace();
                    FrameSelection();
                }
            });
            base.RegisterEventListeners();
        }

       

        public void SetWorld(World world){
            if(this.world != null) world.eventManager.RemoveListener<EntityManagerUpdate>(EntityUpdate);
            this.world = world;
            world.eventManager.AddListener<EntityManagerUpdate>(EntityUpdate);
            VerifyEntities();
            worldLabel.text = world.worldName;
        }

        protected async void EntityUpdate(EntityManagerUpdate args){
            VerifyEntities();
            await Task.Delay(100);
            checkOverlap = true;
        }

        public void LoadSaveData(EntityViewerSaveData data){
            LoadSaveData((NodeViewerSaveData<Entity>)data);
        }

        public void InspectorUpdate(InspectorUpdateEvent args){
            if(checkOverlap == true){
                checkOverlap = false;
                ResolveOverlapping();
            }
            nameUpdateCounter += Time.fixedDeltaTime;
            if(nameUpdateCounter >= nameUpdateGoal){
                nameUpdateCounter = 0;
                UpdateNames();
            }
        }

        protected void UpdateNames(){
            foreach(Node<Entity> node in nodeCache.Values){
                Entity entity = node.key;
                ComponentHandle<Name> nameHandle = world.GetComponent<Name>(entity);
                nodeCache[entity].label.text = nameHandle.component.name;
                if(currentLayer != nodeCache[entity].layer) nodeCache[entity].style.display = DisplayStyle.None;
                else nodeCache[entity].style.display = DisplayStyle.Flex;
            }
        }

        protected void VerifyEntities(){
            if(world != null){
                HashSet<Entity> entities = world.LookUpAllEntities().Select(e => e.entity).ToSet();
                foreach(Entity entity in entities){
                    if(!nodeCache.ContainsKey(entity)){
                        Node<Entity> newNode = new Node<Entity>(entity);
                        nodeCache[entity] = newNode;

                        ComponentHandle<Name> nameHandle = world.GetComponent<Name>(entity);
                        newNode.label.text = nameHandle.component.name;                        

                        anchor.Add(newNode); 

                        newNode.style.left = mouseToAnchorPosition.x;
                        newNode.style.top = mouseToAnchorPosition.y;
                        newNode.lastRestingPosition = mouseToAnchorPosition;
                        
                        newNode.RegisterCallback<MouseDownEvent>(e =>{
                            if(e.button == 0) {
                                StartDraggingNodes(e, newNode);
                            }
                        });

                        newNode.RegisterCallback<MouseUpEvent>(e =>{
                            Rect rect = newNode.AbsoluteRect();
                            lastNodePosition = new Vector2(rect.x, rect.y);

                            newNode.eventManager.Raise(new MouseClickEvent(newNode,e));
                            GameObject entityGO = world.GetComponent<UnityGameObject>(newNode.key)?.component.gameObject; 
                            if(entityGO == null) entityGO = world.AddComponent<UnityGameObject>(newNode.key).component.gameObject;
                            if(e.button == 0){
                                if(entityGO != null){ 
                                    if(hasDraggedNodes == true){
                                        if(!newNode.ClassListContains("selected")){
                                            HashSet<UnityEngine.Object> newSelection = new (Selection.objects){entityGO};
                                            Selection.objects = newSelection.ToArray();
                                        }
                                    }else{
                                        if(newNode.ClassListContains("selected")){
                                            if(e.shiftKey){
                                                HashSet<UnityEngine.Object> newSelection = new (Selection.objects);
                                                newSelection.Remove(entityGO);
                                                Selection.objects = newSelection.ToArray();
                                            }else{
                                                Selection.objects = new UnityEngine.Object[]{entityGO};
                                            }
                                        }else{
                                            if(e.shiftKey){
                                                HashSet<UnityEngine.Object> newSelection = new (Selection.objects){entityGO};
                                                Selection.objects = newSelection.ToArray();
                                            }
                                            else Selection.objects = new UnityEngine.Object[]{entityGO};
                                        }
                                    }                                    
                                }
                            }
                            if(e.button == 1){
                               //select first
                                if(!newNode.ClassListContains("selected") && entityGO != null){
                                    if(e.shiftKey){
                                        HashSet<UnityEngine.Object> newSelection = new (Selection.objects){entityGO};
                                        Selection.objects = newSelection.ToArray();
                                    }
                                    else Selection.objects = new UnityEngine.Object[]{entityGO};
                                }

                                Vector2 position = e.mousePosition - new Vector2(75,65);
                                entityContextMenu.Show(this, position);

                                openButton.eventManager.ClearListeners();
                                openButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                    if(ev.button != 0) return;
                                    EntityEditor.Show(world.LookUpEntity(newNode.key));
                                    entityContextMenu.style.display = DisplayStyle.None;
                                });

                                duplicateButton.eventManager.ClearListeners();
                                duplicateButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                    if(ev.button != 0) return;
                                    ProvenceManagerShortcuts.DuplicateSelection();
                                    entityContextMenu.style.display = DisplayStyle.None;
                                });

                                deleteButton.eventManager.ClearListeners();
                                deleteButton.eventManager.AddListener<MouseClickEvent>(ev => {
                                    if(ev.button != 0) return;
                                    ProvenceManagerShortcuts.RemoveSelection();
                                    entityContextMenu.style.display = DisplayStyle.None;
                                });
                            }

                            if(hasDraggedNodes && !Application.isPlaying){
                                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                            }
                            StopDraggingNodes(e.mousePosition);
                        });
                        
                    }
                }
                
                foreach(Entity entity in nodeCache.Keys.ToSet()){
                    if(!entities.Contains(entity)){
                        anchor.Remove(nodeCache[entity]);
                        nodeCache.Remove(entity);
                    }
                }
            }
        }

        protected void ResolveOverlapping(){
            foreach(Node<Entity> node in nodeCache.Values){
                Node<Entity>[] otherNodes = nodeCache.Values.ToArray();
                for(int i = 0; i < otherNodes.Length; i++){
                    if(node != otherNodes[i]){
                        if(draggingNodes.Contains(node) || draggingNodes.Contains(otherNodes[i])) continue;
                        Rect nodeRect = node.AbsoluteRect();
                        if(nodeRect.Overlaps(otherNodes[i].AbsoluteRect())){
                            node.style.top = nodeRect.y + 30;
                            nodeRect = node.AbsoluteRect();
                            node.lastRestingPosition = new Vector2(nodeRect.x,nodeRect.y);
                            i = 0;
                        }
                    }
                }
            }
            
        }

        public void SelectEntities(ProvenceManagerEditorEntitySelection args){
            foreach(Node<Entity> node in anchor.Query<Node<Entity>>(null,"selected").Build()){
                node.RemoveFromClassList("selected");
            }

            foreach(Entity entity in args.entities){
                if(nodeCache.ContainsKey(entity)) nodeCache[entity].AddToClassList("selected");
            }
        }

        protected void StartDraggingNodes(MouseDownEvent e, Node<Entity> startingNode){
            draggingNodes = new HashSet<Node<Entity>>(){
                startingNode,
                anchor.Query<Node<Entity>>(null, "selected").Build()
            };
            lastMousePosition = e.mousePosition * (1/anchor.style.scale.value.value.x);
            this.RegisterCallback<MouseMoveEvent>(DragNodes);
        }

        protected void DragNodes(MouseMoveEvent e){
            Vector2 offsetPosition = (e.mousePosition * (1/anchor.style.scale.value.value.x)) - lastMousePosition;
            if(offsetPosition.magnitude > 5) hasDraggedNodes = true;
            foreach(Node<Entity> node in draggingNodes){
                Vector2 offset = node.lastRestingPosition + offsetPosition;
                node.style.left = offset.x;
                node.style.top = offset.y;
            }
        }

        protected void StopDraggingNodes(Vector2 mousePosition){
            if(draggingNodes.Count > 0){
                foreach(Node<Entity> node in draggingNodes){
                    node.lastRestingPosition += mousePosition * (1/anchor.style.scale.value.value.x) - lastMousePosition;
                    /* Debug.Log(node.AbsoluteRect().x + anchor.PositionToVector2().x);
                    Debug.Log(node.AbsoluteRect().x + anchor.PositionToVector2().x * (1 + (1 -  anchor.style.scale.value.value.x))); */
                }
            }
            this.UnregisterCallback<MouseMoveEvent>(DragNodes);
            draggingNodes.Clear();
            hasDraggedNodes = false;
            checkOverlap = true;
        }

        protected void StartBoxSelection(MouseDownEvent e){
            selectionSquare.SetPosition(e.localMousePosition);
            boxSelectionStartingPosition = e.localMousePosition;
            this.RegisterCallback<MouseMoveEvent>(BoxSelectionStep);
        }

        protected void BoxSelectionStep(MouseMoveEvent e){
            Vector2 size = boxSelectionStartingPosition - e.localMousePosition;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            
            if(size.magnitude > 20) selectionSquare.style.display = DisplayStyle.Flex;
            else selectionSquare.style.display = DisplayStyle.None;

            if(boxSelectionStartingPosition.x < e.localMousePosition.x){
                selectionSquare.style.left = boxSelectionStartingPosition.x;                
            }else{
                selectionSquare.style.left = boxSelectionStartingPosition.x - size.x;
            }
            selectionSquare.style.width = size.x;

            if(boxSelectionStartingPosition.y < e.localMousePosition.y){
                selectionSquare.style.top = boxSelectionStartingPosition.y;
            }else{
                selectionSquare.style.top = boxSelectionStartingPosition.y - size.y;
            }
            selectionSquare.style.height = size.y;
        }

        protected void StopBoxSelection(){
            this.UnregisterCallback<MouseMoveEvent>(BoxSelectionStep);
            selectionSquare.style.display = DisplayStyle.None;
        }

        protected bool CanBoxSelect(){
            Rect rect = selectionSquare.AbsoluteRect();
            if(rect.width >= 20) return true;
            if(rect.height >= 20) return true;
            return false;
        }

        protected void BoxSelect(MouseUpEvent e){
            if(CanBoxSelect()){
                Vector2 anchorPosition = anchor.PositionToVector2(); 

                HashSet<UnityEngine.Object> newSelection = new();                
                if(e.shiftKey) newSelection.Add(Selection.objects);
                selectionSquare.style.left = (selectionSquare.style.left.value.value * (1/anchor.style.scale.value.value.x)) - (anchorPosition.x * (1/anchor.style.scale.value.value.x));
                selectionSquare.style.top = (selectionSquare.style.top.value.value * (1/anchor.style.scale.value.value.x)) - (anchorPosition.y * (1/anchor.style.scale.value.value.x));
                Rect boxRect = selectionSquare.AbsoluteRect();
                boxRect.width *= 1/anchor.style.scale.value.value.x;
                boxRect.height *= 1/anchor.style.scale.value.value.x;
                foreach(Node<Entity> node in nodeCache.Values){
                    Rect nodeRect = node.AbsoluteRect();
                    //nodeRect.x += anchorPosition.x * anchor.style.scale.value.value.x;
                    //nodeRect.y += anchorPosition.y;
                    if(nodeRect.Overlaps(boxRect)){
                        GameObject go = world.GetComponent<UnityGameObject>(node.key)?.component.gameObject;
                        if(go != null) newSelection.Add(go);
                    }
                }

                Selection.objects = newSelection.ToArray();
            }
        }

        protected void ArrangeByWorldSpace(){
            List<Node<Entity>> selectedNodes = new();
            anchor.Query<Node<Entity>>(null,"selected").ForEach( node => {selectedNodes.Add(node);});
            if(selectedNodes.Count == 0) return;
            Vector2 furthestOut = selectedNodes.First().PositionToVector2();

            Dictionary<int,Dictionary<int,Dictionary<int,Node<Entity>>>> positions = new();

            int? highestX = null;

            foreach(Node<Entity> node in selectedNodes){
                if(world.GetComponent<UnityGameObject>(node.key)?.component.gameObject is GameObject go && go != null){
                    Vector3 pos = go.transform.position.Snap();
                    if(!positions.ContainsKey((int)pos.y)) positions[(int)pos.y] = new();
                    if(!positions[(int)pos.y].ContainsKey((int)pos.z)) positions[(int)pos.y][(int)pos.z] = new();
                    positions[(int)pos.y][(int)pos.z][(int)pos.x] = node;
                    if(highestX == null || highestX < pos.x) highestX = (int)pos.x;
                }
                Vector2 position = node.PositionToVector2();
                if(position.x < furthestOut.x) furthestOut.x = position.x;
                if(position.y < furthestOut.y) furthestOut.y = position.y;
            }
            float currentY = furthestOut.y;

            positions.Sort();
            int minY = positions.First().Key;
            int maxY = positions.Last().Key;
            for(int y = maxY; y >= minY; y--){
                if(positions.ContainsKey(y)){
                    positions[y].Sort();
                    int minZ = positions[y].First().Key;
                    int maxZ = positions[y].Last().Key;  
                    //float groupY = furthestOut.y + 
                    for(int z = minZ; z <= maxZ; z++){
                        if(positions[y].ContainsKey(z)){
                            positions[y][z].Sort();
                            int minX = positions[y][z].First().Key;
                            int maxX = positions[y][z].Last().Key;
                            for(int x = minX; x <= maxX; x++){   
                                if(positions[y][z].ContainsKey(x)){
                                    int diffZ = z - minZ;
                                    int diffX = x - (int)highestX;
                                    Vector2 newPosition = new(){
                                        x = furthestOut.x + (gridSize.x * (maxX - x + ((int)highestX - maxX))),
                                        y = currentY + (gridSize.y * diffZ)
                                        //y = currentY + (gridSize.y * zCount)
                                    };
                                    positions[y][z][x].style.left = newPosition.x;
                                    positions[y][z][x].style.top = newPosition.y;
                                    positions[y][z][x].lastRestingPosition = newPosition;
                                }
                            }                            
                        }
                    }
                    currentY += gridSize.y * (maxZ - minZ + 2);                                    
                    //currentY += gridSize.y * (positions[y].Count + 2);
                }
            }
        }

        public EntityViewerSaveData GetSaveData(){
            return new EntityViewerSaveData(nodeCache.Values.ToArray()){
                anchorPosition = anchor.PositionToVector2(),
                scale = anchor.style.scale.value.value.x
            };
        }
        
    }

}