using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }
      
        public ListItem(bool alternate = false, bool selectable = false, bool hoverable = false){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("list-item");
            if(alternate) this.AddToClassList("alternate");
            if(selectable) this.AddToClassList("selectable");
            if(hoverable) this.AddToClassList("hoverable");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
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
            
            objectDisplay = new ObjectField();
            objectDisplay.allowSceneObjects = false;
            objectDisplay.objectType = typeof(GameObject);
            objectDisplay.value = objectValue;
            objectDisplay.AddToClassList("list-item-input","list-item-object-input","second-alternate");

            button = new ListItemImage(caretIcon);
            button.AddToClassList("selectable","hoverable");

            button.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                HierarchySelector hierarchySelector = HierarchySelector.Open();
                hierarchySelector.eventManager.Raise<SetSelectorParameters<GameObject>>(new SetSelectorParameters<GameObject>(root));
                hierarchySelector.eventManager.AddListener<MainframeKeySelection<int[]>>(ev =>{
                    this.value = ev.value;
                    this.objectDisplay.value = HierarchySelector.GetChildByHierarhcy(root, ev.value);
                    this.eventManager.Raise<MainframeKeySelection<int[]>>(ev);
                });
            });

            this.Add(label, objectDisplay, button);
        }
    }

}