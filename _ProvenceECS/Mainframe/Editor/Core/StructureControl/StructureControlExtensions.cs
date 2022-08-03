using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{

    public partial class StructureControl<T> : VisualElement {

        protected static void CreateControl<V>(StructureControl<HashSet<V>> control){
            Div container = new Div();
            ListItem addBar = new ListItem().AddToClassList("spacer","alternate");
            addBar.AddDiv().AddToClassList("filler");
            addBar.AddImage(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/plus.png"));
            addBar.eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                control.structure.Add(default(V));
                control.eventManager.Raise(new StructureControlUpdated<HashSet<V>>(control));
                DrawSetControls(control, container);
            });
            control.Add(container,addBar);
            DrawSetControls(control, container);
        }

        protected static void DrawSetControls<V>(StructureControl<HashSet<V>> control, Div container){
            container.Clear();
            foreach(V item in control.structure){
                if(control.specifiedFields.Contains(typeof(V))){
                    FieldControl<V> fieldControl = new FieldControl<V>(item, control.name + "list-item","item",false, control.world);
                    fieldControl.eventManager.AddListener<FieldControlUpdated<V>>(e => {
                        control.structure.Remove(item);
                        control.structure.Add(e.control.value);
                        control.eventManager.Raise(new StructureControlUpdated<HashSet<V>>(control));
                    });
                    ListItemImage delButton = fieldControl.AddImage(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png")).AddToClassList("selectable","hoverable","icon");
                    delButton.eventManager.AddListener<MouseClickEvent>(e => {
                        control.structure.Remove(item);
                        control.eventManager.Raise(new StructureControlUpdated<HashSet<V>>(control));
                        DrawSetControls<V>(control, container);
                    });
                    container.Add(fieldControl);
                }else{
                    V structure = item;
                    StructureControl<V> structureControl = new StructureControl<V>(ref structure);
                    structureControl.eventManager.AddListener<StructureControlUpdated<V>>(e => {
                        control.eventManager.Raise(new StructureControlUpdated<HashSet<V>>(control));
                    });
                }
            }
        }

        protected static void CreateControl<V>(StructureControl<List<V>> control){
            Div container = new Div();
            ListItem addBar = new ListItem().AddToClassList("spacer","alternate");
            addBar.AddDiv().AddToClassList("filler");
            addBar.AddImage(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/plus.png"));
            addBar.eventManager.AddListener<MouseClickEvent>(e => {
                if(e.button != 0) return;
                control.structure.Add(default(V));
                control.eventManager.Raise(new StructureControlUpdated<List<V>>(control));
                DrawListControls(control, container);
            });
            control.Add(container,addBar);
            DrawListControls(control, container);
        }

        protected static void DrawListControls<V>(StructureControl<List<V>> control, Div container){
            container.Clear();
            for(int i = 0; i < control.structure.Count; i++){
                int index = i;
                if(control.specifiedFields.Contains(typeof(V))){
                    FieldControl<V> fieldControl = new FieldControl<V>(control.structure[index], control.name + "list-item","item",false,control.world);
                    fieldControl.eventManager.AddListener<FieldControlUpdated<V>>(e => {
                        control.structure[index] = e.control.value;
                        control.eventManager.Raise(new StructureControlUpdated<List<V>>(control));
                    });
                    ListItemImage delButton = fieldControl.AddImage(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png")).AddToClassList("selectable","hoverable","icon");
                    delButton.eventManager.AddListener<MouseClickEvent>(e => {
                        control.structure.RemoveAt(index);
                        control.eventManager.Raise(new StructureControlUpdated<List<V>>(control));
                        DrawListControls<V>(control, container);
                    });
                    container.Add(fieldControl);
                }else{
                    V structure = control.structure[index];
                    StructureControl<V> structureControl = new StructureControl<V>(ref structure);
                    structureControl.eventManager.AddListener<StructureControlUpdated<V>>(e => {
                        control.eventManager.Raise(new StructureControlUpdated<List<V>>(control));
                    });
                }
            }
        }

        protected static void CreateControl<V>(StructureControl<ProvenceCollectionInstance<V>> control) where V : ProvenceCollectionEntry{
            //Change to picker
            ListItem item = new ListItem();
            KeySelectorElement keySelector = item.AddKeySelector("Entry Name:",control.structure.key,ProvenceManager.Collections<V>().Keys.ToSet());
            keySelector.eventManager.AddListener<MainframeKeySelection<string>>(e=>{
                control.structure.key = e.value;
                control.eventManager.Raise<MainframeKeySelection<string>>(new MainframeKeySelection<string>(e.value));
                control.eventManager.Raise(new StructureControlUpdated<ProvenceCollectionInstance<V>>(control));
            });
            item.AddButton("Set to Entry").eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(control.world != null && control.entity != null){
                    control.world.eventManager.Raise(new SetEntityToManualEntry<V>(control.entity, keySelector.value));
                    control.eventManager.Raise(new StructureControlUpdated<ProvenceCollectionInstance<V>>(control));
                    control.eventManager.Raise(new StructureControlRefreshRequest(100));
                }
            });
            control.Add(item);
        }

        protected static void CreateControl(StructureControl<Model> control){
            ListItem keySelectorItem = new ListItem();
            KeySelectorElement keySelector = keySelectorItem.AddKeySelector("Model Bank Key", control.structure.manualKey, new HashSet<string>(ProvenceManager.ModelBank.Keys));
            keySelector.eventManager.AddListener<MainframeKeySelection<string>>(e =>{
                control.structure.manualKey = e.value;
                control.eventManager.Raise<MainframeKeySelection<string>>(new MainframeKeySelection<string>(e.value));
                control.eventManager.Raise<StructureControlUpdated<Model>>(new StructureControlUpdated<Model>(control));
            });
            keySelector.eventManager.AddListener<ListItemInputCancel>(e => {
                control.structure.manualKey = "";
                control.eventManager.Raise<ListItemInputCancel>(new ListItemInputCancel(control));
                control.eventManager.Raise<StructureControlUpdated<Model>>(new StructureControlUpdated<Model>(control));
            });
            control.Add(keySelectorItem);
        }

    }

}