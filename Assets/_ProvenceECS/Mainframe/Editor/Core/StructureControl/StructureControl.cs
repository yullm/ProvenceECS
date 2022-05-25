using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public class StructureControlRefreshRequest : MainframeUIArgs{
        public int delay;
        public StructureControlRefreshRequest(int delay = 0){
            this.delay = delay;
        }
    }

    public class StructureControlUpdated<T> : MainframeUIArgs{
        StructureControl<T> control;
        public StructureControlUpdated(StructureControl<T> control){
            this.control = control;
        }
    }

    public partial class StructureControl<T> : VisualElement{

        public List<Type> specifiedFields = new List<Type>(){
            typeof(bool),
            typeof(byte),
            typeof(ushort),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(GameObject),
            typeof(Entity)
        };

        protected int depth;
        public World world;
        public Entity entity;
        public T structure;
        private bool startAlt;
        protected List<Type> attributeHideList;
        public EventManager<MainframeUIArgs> eventManager;
        
        public StructureControl(ref T structure, string name = null, bool startAlt = false, World world = null, Entity entity = null, int depth = 0, params Type[] attributeHideList):base(){
            this.structure = structure;
            this.eventManager = new EventManager<MainframeUIArgs>();
            if(name == null) name = typeof(T).Name;
            this.name = name.ToLower() + "-control";
            this.startAlt = startAlt;
            this.depth = depth;
            this.world = world;
            this.entity = entity;
            this.attributeHideList = new List<Type>(attributeHideList);
            this.attributeHideList.Add(typeof(DontDisplayInEditor));
            InitControl();
        }

        private void InitControl(){
            this.AddToClassList("structure-control");
            CreateControl((dynamic)this);
        }

        /* protected void CreateDefaultControl(){
            bool alternate = startAlt;
            FieldInfo[] fields = structure.GetType().GetFields();
            for(int i = 0; i < fields.Length; i++){
                try{
                    Type fieldType = fields[i].FieldType;
                    bool hide = false;
                    foreach(Type attributeType in attributeHideList){
                        if(fields[i].IsDefined(attributeType,false) || fieldType.IsDefined(attributeType,false)){
                            hide = true;
                            break;
                        }
                    }
                    if(hide) continue;
                    if(specifiedFields.Contains(fieldType) || fieldType.IsDefined(typeof(CustomFieldControl), false) || fieldType.IsEnum)
                        Helpers.InvokeGenericMethod(this,"CreateFieldControl", fieldType, fields[i], alternate);
                    else
                        Helpers.InvokeGenericMethod(this,"CreateNestedStructureControl", fieldType, fields[i]);
                    alternate = !alternate;
                }catch(Exception e){
                    Debug.LogWarning(e);
                }
            }
        } */

        protected static void CreateControl<U>(StructureControl<U> control){
            bool alternate = control.startAlt;
            FieldInfo[] fields = control.structure.GetType().GetFields();
            for(int i = 0; i < fields.Length; i++){
                try{
                    Type fieldType = fields[i].FieldType;
                    bool hide = false;
                    foreach(Type attributeType in control.attributeHideList){
                        if(fields[i].IsDefined(attributeType,false) || fieldType.IsDefined(attributeType,false)){
                            hide = true;
                            break;
                        }
                    }
                    if(hide) continue;
                    if(control.specifiedFields.Contains(fieldType) || fieldType.IsDefined(typeof(CustomFieldControl), false) || fieldType.IsEnum)
                        Helpers.InvokeGenericMethod(control,"CreateFieldControl", fieldType, fields[i], alternate);
                    else
                        Helpers.InvokeGenericMethod(control,"CreateNestedStructureControl", fieldType, fields[i]);
                    alternate = !alternate;
                }catch(Exception e){
                    Debug.LogWarning(e);
                }
            }
        }

        protected void CreateFieldControl<U>(FieldInfo info, bool alternate = false){            
            FieldControl<U> el = new FieldControl<U>((U)info.GetValue(structure), this.name + "--" + info.Name, info.Name, alternate, world);
            el.eventManager.AddListener<FieldControlUpdated<U>>(e => {
                info.SetValue(structure, el.value);
                eventManager.Raise<StructureControlUpdated<T>>(new StructureControlUpdated<T>(this));
            });
            this.Add(el); 
            
        }

        protected void CreateNestedStructureControl<U>(FieldInfo info){
            if(depth >= 5){
                Debug.LogWarning("Nested too deep");
                return;
            }
            string title = System.Text.RegularExpressions.Regex.Replace(info.Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
            ListItem titleItem = new ListItem();
            titleItem.AddTitle(title);
            titleItem.name = this.name + "--" + typeof(U).Name.ToLower() + "-control-title";
            titleItem.AddToClassList("structure-control-title","spacer");
            U structRef = (U)info.GetValue(structure);
            if(structRef == null) return;
            StructureControl<U> el = new StructureControl<U>(ref structRef, this.name + "--" + typeof(U).Name.ToLower(), startAlt, world, entity, depth+1, attributeHideList.ToArray());
            el.eventManager.AddListener<StructureControlUpdated<U>>(e =>{
                eventManager.Raise<StructureControlUpdated<T>>(new StructureControlUpdated<T>(this));
            });
            this.Add(titleItem);
            this.Add(el);
        }

    }

}