﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProvenceECS.Mainframe{
    //build parser
    public class AssetManagerEditor : MainframeTableWindow<string>{

        protected ColumnScroller pairListScroller;
        protected Texture delButtonIcon;

        public override void OnEnable(){
            this.titleContent = new GUIContent("Provence Asset Manager");
            LoadTree(UIDirectories.GetPath("asset-manager","uxml"), UIDirectories.GetPath("asset-manager","uss"));
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            delButtonIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
            pairListScroller = root.Q<ColumnScroller>("pair-list-scroller");
            eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
        }

        protected override void RegisterEventListeners(){
            if(root == null) return;
            eventManager.AddListener<DrawColumnEventArgs<string>>(DrawColumn);
            eventManager.AddListener<SceneSavedEvent>(SaveManager);
            root.Q<ListItemImage>("add-pair-button").eventManager.AddListener<MouseClickEvent>(AddPair);
        }

        protected override void DrawColumn(DrawColumnEventArgs<string> args){
            switch(args.column){
                case 0:
                    DrawPairList();
                    break;
            }
        }

        protected void DrawPairList(){
            pairListScroller.Clear();
            bool alternate = false;
            ProvenceManager.AssetManager.addressCache.Sort();
            foreach(KeyValuePair<string,AssetData> pair in ProvenceManager.AssetManager.addressCache){
                ListItem item = new ListItem(alternate);

                item.AddLabel("Address:",true);
                ListItemTextInput addressInput = item.AddTextField(pair.Key);
                item.AddLabel("Resource:",true).AddToClassList("asset-label");
                ObjectField objectField = item.AddObjectField(typeof(Object));
                if(!pair.Value.Equals("")) objectField.value = ProvenceManager.AssetManager.LoadAsset<Object>(pair.Key);
                item.AddLabel("Offset:",true).AddToClassList("asset-label");
                Vector3Field offsetField = item.AddVector3Field(ProvenceManager.AssetManager.addressCache[pair.Key].offset);
                ListItemImage delButton = item.AddImage(delButtonIcon);
                delButton.AddToClassList("icon","selectable","hoverable");

                addressInput.eventManager.AddListener<ListItemInputCommit>(e => {
                    if(addressInput.text != pair.Key){
                        ProvenceManager.AssetManager.ChangeKey(pair.Key,addressInput.text);
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
                    }                    
                });
                addressInput.eventManager.AddListener<LoseFocus>(e =>{                    
                    if(addressInput.text != pair.Key){
                        ProvenceManager.AssetManager.ChangeKey(pair.Key,addressInput.text);
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
                    }   
                });
                addressInput.eventManager.AddListener<ListItemInputCancel>(e =>{
                    addressInput.value = pair.Key;
                });

                item.eventManager.AddListener<ListItemInputChange>(e => {
                    if(e.input == objectField){
                        string assetPath = AssetDatabase.GetAssetPath(objectField.value);
                        if(objectField.value == null){
                            ProvenceManager.AssetManager.addressCache[pair.Key].resourcePath = "";
                            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        }
                        if(assetPath.Contains(@"/Resources/")){
                            ProvenceManager.AssetManager.SetResourceByPath(pair.Key,assetPath);
                            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                        }else{                            
                            if(objectField.value != null){ 
                                Debug.LogWarning("Not Resource");
                                objectField.value = null;
                            }
                        }
                    }
                    if(e.input == offsetField){
                        ProvenceManager.AssetManager.addressCache[pair.Key].offset = offsetField.value;
                        eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    }
                });

                delButton.eventManager.AddListener<MouseClickEvent>(e =>{
                    if(e.button != 0) return;
                    ProvenceManager.AssetManager.addressCache.Remove(pair.Key);
                    eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
                    eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
                });

                pairListScroller.Add(item);
                alternate = !alternate;
            }
        }

        protected void AddPair(MouseClickEvent e){
            if(e.button != 0) return;
            ProvenceManager.AssetManager.AddPair("","");
            eventManager.Raise<SetSceneDirtyEvent>(new SetSceneDirtyEvent(SceneManager.GetActiveScene()));
            eventManager.Raise<DrawColumnEventArgs<string>>(new DrawColumnEventArgs<string>(0));
        }

        protected void SaveManager(SceneSavedEvent args){
            ProvenceManager.AssetManager.Save();
        }
    }
}