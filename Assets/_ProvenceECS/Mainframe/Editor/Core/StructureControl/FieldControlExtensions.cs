using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public static class FieldControlExtensions{

        public static void DrawControl(this FieldControl<bool> control){
            EnumField input = control.AddBoolean(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = (BooleanEnum)input.value == BooleanEnum.TRUE ? true : false;
                control.eventManager.Raise<FieldControlUpdated<bool>>(new FieldControlUpdated<bool>(control));
            });
        }

        public static void DrawControl(this FieldControl<int> control){
            IntegerField input = control.AddIntField(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<int>>(new FieldControlUpdated<int>(control));
            });
        }

        public static void DrawControl(this FieldControl<float> control){
            FloatField input = control.AddFloatField(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<float>>(new FieldControlUpdated<float>(control));
            });
        }

        public static void DrawControl(this FieldControl<string> control){
            TextField input = control.AddTextField(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<string>>(new FieldControlUpdated<string>(control));
            });
        }

        public static void DrawControl(this FieldControl<Vector3> control){
            Vector3Field input = control.AddVector3Field(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<Vector3>>(new FieldControlUpdated<Vector3>(control));
            });
        }

        public static void DrawControl(this FieldControl<Vector2> control){
            Vector2Field input = control.AddVector2Field(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<Vector2>>(new FieldControlUpdated<Vector2>(control));
            });
        }

        public static void DrawControl(this FieldControl<GameObject> control){
            ObjectField input = control.AddObjectField(typeof(GameObject),false);
            input.value = control.value;
            control.eventManager.AddListener<ListItemInputChange>(e =>{
                if(e.input != input) return;
                control.value = (GameObject)input.value;
                control.eventManager.Raise<FieldControlUpdated<GameObject>>(new FieldControlUpdated<GameObject>(control));
            });
        }

        public static void DrawControl(this FieldControl<Color> control){
            ColorField input = control.AddColorField(control.value);
            control.eventManager.AddListener<ListItemInputChange>(e => {
                if(e.input != input) return;
                control.value = input.value;
                control.eventManager.Raise<FieldControlUpdated<Color>>(new FieldControlUpdated<Color>(control));
            });
        }

        public static void DrawControl(this FieldControl<Entity> control){
            if(control.world == null) return;
            ListItemText input = control.AddEntitySelector(control.world, control.value);
            control.eventManager.AddListener<ListItemInputChange>(e => {
                if(e.input != input) return;
                control.value = (Entity)input.text;
                control.eventManager.Raise<FieldControlUpdated<Entity>>(new FieldControlUpdated<Entity>(control));
            });
        }

    }

}