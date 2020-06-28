using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    //Events

    public class MouseClickEvent : MainframeUIArgs{
        public VisualElement element;
        public int button;
        public Vector2 mousePosition;
        public MouseClickEvent(VisualElement element, int button, Vector2 mousePosition){
            this.element = element;
            this.button = button;
            this.mousePosition = mousePosition;
        }
    }
    
    public class KeyPressEvent : MainframeUIArgs{

        public VisualElement element;
        public KeyCode keyCode;

        public KeyPressEvent(VisualElement element, KeyCode keyCode){
            this.element = element;
            this.keyCode = keyCode;
        }

    }

    public class ListItemInputChange : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputChange(VisualElement input){
            this.input = input;
        }
    }

    public class ListItemInputCommit : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputCommit(VisualElement input){
            this.input = input;
        }
    }

    public class ListItemInputCancel : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputCancel(VisualElement input){
            this.input = input;
        }
    }

    public class LoseFocus : MainframeUIArgs{
        public VisualElement input;
        public LoseFocus(VisualElement input){
            this.input = input;
        }
    }

    //Elements

    public class ListItem : VisualElement{

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

        public Div AddDiv(){
            Div el = new Div();
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

        public IntegerField AddIntField(int value = 0, bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
            IntegerField el = new IntegerField();
            el.value = value;
            el.AddToClassList("list-item-input");
            el.AddToClassList("list-item-int-input");
            AddAlternates(el, alternate, secondAlternate, thirdAlternate);
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
            el.eventManager.AddListener<ListItemInputChange>(e =>{
                eventManager.Raise<ListItemInputChange>(new ListItemInputChange(el));
            });
            this.Add(el);
            return el;
        }

        public Vector3Field AddVector3Field(Vector3 value = new Vector3(),bool alternate = false, bool secondAlternate = false, bool thirdAlternate = false){
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
                    GameObject go = handle.GetGameObject();
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

}