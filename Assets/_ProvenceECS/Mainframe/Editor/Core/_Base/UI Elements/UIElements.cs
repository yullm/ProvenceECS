﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

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

}
