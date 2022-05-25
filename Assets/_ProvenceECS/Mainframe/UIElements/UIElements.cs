using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public abstract class MainframeUIArgs {}

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


    public class Wrapper : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<Wrapper> {}

        public Wrapper(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("wrapper");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    public class Div : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<Div> {}

        public Div(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    public class MenuBar : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<MenuBar> {}

        public MenuBar(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("menu-bar");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    public class TableWrapper : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<TableWrapper> {}

        public TableWrapper(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("table-wrapper");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    public class PageColumn : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<PageColumn> {}

        public PageColumn(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("page-column");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    public class ColumnScroller : ScrollView{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ColumnScroller> {}

        public ColumnScroller(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("column-scroller");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

        public ColumnScroller(bool alternate) : this(){
            if(alternate) this.AddToClassList("alternate");
        }

    }

    public class DropDownMenu : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<DropDownMenu> {}

        public DropDownMenu(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("drop-down-menu");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
            this.RegisterCallback<MouseLeaveEvent>(e => {
                this.style.display = DisplayStyle.None;
            });
        }

        public void Show(VisualElement root, MouseClickEvent e = null, bool changePosition = false){

            root.Query<DropDownMenu>().ForEach(current =>{
                current.style.display = DisplayStyle.None;
            });                

            if(changePosition){
                this.style.top = StyleKeyword.Auto;
                this.style.bottom = StyleKeyword.Auto;
                this.style.left = StyleKeyword.Auto;
                this.style.right = StyleKeyword.Auto;

                bool top = (e.mousePosition.y < (root.contentRect.height / 2));
                bool left = (e.mousePosition.x < (root.contentRect.width / 2));
                
                if(left){
                    this.style.left = e.mousePosition.x - 20 > 0 ? e.mousePosition.x - 20 : 0;
                }else{
                    float rightX = root.contentRect.width - e.mousePosition.x;
                    this.style.right = rightX - 20 > 0 ? rightX - 20 : 0;
                }
                if(top){
                    this.style.top = e.mousePosition.y - 20;
                }else{
                    this.style.bottom = e.mousePosition.y * -1;
                }              
                
            }
            this.style.display = DisplayStyle.Flex;
        }

    }

    public class img : VisualElement{

        public Texture image;
        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<img, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "src" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return null;
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((img)ve).SetImage(Resources.Load<Texture>(textAttribute.GetValueFromBag(bag, cc)));
            }
        }

        public img(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

        public img(string resource):this(){
            SetImage(Resources.Load<Texture>(resource));
        }

        public img SetImage(Texture texture){
            this.image = texture;
            this.style.backgroundImage = (StyleBackground)image;
            return this;
        }

    }

    public class ProvenceText : TextElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ProvenceText, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "text" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ProvenceText)ve).text = textAttribute.GetValueFromBag(bag, cc);
            }
        }

        public ProvenceText() : base(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

        public ProvenceText(string text) : base(){
            this.text = text;
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e.button,e.mousePosition));
            });
        }

    }

    

}
