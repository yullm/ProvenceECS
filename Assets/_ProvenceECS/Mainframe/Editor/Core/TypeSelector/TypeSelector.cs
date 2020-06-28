using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ProvenceECS.Mainframe{
    
    public class TypeSelector : MainframeSelectorWindow<Type>{

        public class TypeSelectorParameters{
            public Type baseType;
            public bool showBase;
            public List<Type> existingTypes;
            public Type[] additionalAssemblies;

            public TypeSelectorParameters(Type baseType, bool showBase = false, List<Type> existingTypes = null, params Type[] additionalAssemblies){
                this.baseType = baseType;
                this.showBase = showBase;
                this.existingTypes = existingTypes;
                this.additionalAssemblies = additionalAssemblies;
            }
        }

        private TypeSelectorParameters parameters;
        private string searchString = "";

        public static void Open(TypeSelectorParameters args, ProvenceDelegate<MainframeKeySelection<Type>> callback){
            TypeSelector.Close<TypeSelector>();
            TypeSelector window = MainframeSelectorWindow<Type>.Open<TypeSelector>("Type Selector", callback);
            window.eventManager.Raise<SetSelectorParameters<TypeSelectorParameters>>(new SetSelectorParameters<TypeSelectorParameters>(args));
        }

        public override void OnEnable(){
            LoadTree(UIDirectories.GetPath("type-selector","uxml"),UIDirectories.GetPath("type-selector","uss"));
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SetSelectorParameters<TypeSelectorParameters>>(SetParameters);
            eventManager.AddListener<DrawColumnEventArgs<Type>>(DrawColumn);
            root.Q<TextField>("search-bar").RegisterValueChangedCallback(e=>{
                searchString = ((TextField)e.target).text;
                eventManager.Raise<DrawColumnEventArgs<Type>>(new DrawColumnEventArgs<Type>(0));
            });
            root.Q<VisualElement>("clear-search-button").RegisterCallback<MouseUpEvent>(e => {
                if(e.button != 0) return;
                root.Q<TextField>("search-bar").value = "";
            });
        }

        protected override void InitializeWindow(){}

        protected void SetParameters(SetSelectorParameters<TypeSelectorParameters> args){
            parameters = args.paramaters;
            GenerateTypeList();
        }

        protected override void DrawColumn(DrawColumnEventArgs<Type> args){
            switch(args.column){
                case 0:
                    SortListBySearch();
                    break;
            }
        }

        protected void SortListBySearch(){
            ScrollView scroller = root.Q<ScrollView>("type-list");
            scroller.Query<ListItem>().ForEach(item => {
                item.style.display = DisplayStyle.Flex;
            });

            if(!searchString.Equals("")){
                scroller.Query<ListItem>().ForEach(item => {
                    if(!item.Q<TextElement>().text.ToLower().Contains(searchString.ToLower())) item.style.display = DisplayStyle.None; 
                });
            }
        }

        protected void GenerateTypeList(){
            List<Type> types = Helpers.GetAllTypesFromBaseType(parameters.baseType, parameters.additionalAssemblies);
            ScrollView scroller = root.Q<ScrollView>("type-list");
            bool alternate = false;
            foreach(Type type in types){
                if(parameters.existingTypes.Contains(type)) continue;
                if(type.IsDefined(typeof(DontDisplayInEditor),false)) continue;
                if(type == parameters.baseType && !parameters.showBase) continue;
                ListItem item = new ListItem(alternate);
                string typeName = System.Text.RegularExpressions.Regex.Replace(type.Name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
                ListItemText button = item.AddButton(typeName,false,false,true);

                scroller.Add(item);
                alternate = !alternate;

                button.eventManager.AddListener<MouseClickEvent>(e =>{
                    if(e.button != 0) return;
                    if(item.ClassListContains("selected")){
                        ReturnSelection();
                    }else{
                        scroller.Query<ListItem>(null,"selected").ForEach(current =>{
                            current.RemoveFromClassList("selected");
                        });
                        item.AddToClassList("selected");
                        chosenKey = type;
                    }
                });
            }
        }

    }

}