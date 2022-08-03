using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class FieldControlUpdated<T> : MainframeUIArgs{
        public FieldControl<T> control;
        public FieldControlUpdated(FieldControl<T> control){
            this.control = control;
        }
    }

    public class FieldControl<T> : ListItem{
        public World world;
        public T value;
        protected string label;
        protected bool alternate;
        
        public FieldControl(T value, string name, string label = "", bool alternate = false, World world = null):base(alternate){
            this.value = value;
            this.name = name;
            this.label = System.Text.RegularExpressions.Regex.Replace(label, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
            this.alternate = alternate;
            this.world = world;
            DrawListItem();            
        }

        protected void DrawListItem(){
            if(alternate) this.AddToClassList("alternate"); 
            this.AddToClassList("field-control");
            AddLabel(label, true);
            if(typeof(T).IsEnum) InitEnumControl();
            else InitControl();
        }

        protected void InitControl(){
            try{
                Helpers.InvokeExtensionMethod(this, "DrawControl", typeof(FieldControlExtensions));
            }catch(Exception e){
                Debug.LogWarning("Missing extension for type: " + typeof(T) +"; " + e);
            }
        }
        
        protected void InitEnumControl(){
            EnumField input = AddEnumField(value as Enum);
            eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                value = (T)Enum.Parse(typeof(T),input.value.ToString());
                eventManager.Raise<FieldControlUpdated<T>>(new FieldControlUpdated<T>(this));
            });
        }

    }
}
