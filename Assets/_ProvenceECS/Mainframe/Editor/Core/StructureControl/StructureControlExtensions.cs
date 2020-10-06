using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ransacked;
using Ransacked.Mainframe;
using UnityEditor;

namespace ProvenceECS.Mainframe{

    public static class StructureControlExtensions {
        public static void CreateControl(this StructureControl<ActorManualInstance> control){
            //Change to picker
            ListItem item = new ListItem();
            item.AddLabel("Entry Name:",true);
            ListItemTextInput nameInput = item.AddTextField(control.structure.key);
            nameInput.eventManager.AddListener<ListItemInputChange>(e =>{
                control.structure.key = nameInput.text;
                control.eventManager.Raise<StructureControlUpdated<ActorManualInstance>>(new StructureControlUpdated<ActorManualInstance>(control));
            });
            item.AddButton("Set to Entry").eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(control.world != null && control.entity != null){
                    control.world.eventManager.Raise<SetEntityToManualEntry>(new SetEntityToManualEntry(control.entity));
                    control.eventManager.Raise<StructureControlUpdated<ActorManualInstance>>(new StructureControlUpdated<ActorManualInstance>(control));
                    control.eventManager.Raise<StructureControlRefreshRequest>(new StructureControlRefreshRequest(100));
                }
            });
            control.Add(item);
        }
    
        public static void CreateControl(this StructureControl<Model> control){
            ListItem keySelectorItem = new ListItem();
            KeySelectorElement keySelector = keySelectorItem.AddKeySelector("Model Bank Key",control.structure.manualKey, new HashSet<string>(RansackedMainframe.ModelBank.Keys));
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

        public static void CreateControl(this StructureControl<SoundBank> control){
            Texture addIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/plus.png");
            Div container = new Div();
            ListItemSearchBar searchBar = new ListItemSearchBar(container);
            
            ListItemImage addButton = searchBar.AddImage(addIcon);
            addButton.AddToClassList("selectable","hoverable");
            addButton.eventManager.AddListener<MouseClickEvent>(e =>{
                if(e.button != 0) return;
                if(!control.structure.soundBank.ContainsKey("")){
                    control.structure.soundBank[""] = "";
                    control.eventManager.Raise<StructureControlUpdated<SoundBank>>(new StructureControlUpdated<SoundBank>(control));
                    DrawSoundBankList(control, container);                    
                }
            });

            DrawSoundBankList(control, container);

            control.Add(searchBar,container);
        }

        public static void DrawSoundBankList(StructureControl<SoundBank> control, Div container){
            Texture delIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Icons/times.png");
            container.Clear();
            bool alternate = false;
            foreach(KeyValuePair<string,string> kvp in control.structure.soundBank){
                ListItem item = new ListItem(alternate);
                item.AddToClassList("search-list-item");
                item.userData = kvp.Key;

                KeySelectorElement keySelector = item.AddKeySelector("Key", kvp.Key, SoundBankKeys.keys);
                keySelector.eventManager.AddListener<MainframeKeySelection<string>>(e => {
                    string temp = control.structure.soundBank[kvp.Key];
                    control.structure.soundBank[e.value] = temp;
                    control.structure.soundBank.Remove(kvp.Key);
                    control.eventManager.Raise<StructureControlUpdated<SoundBank>>(new StructureControlUpdated<SoundBank>(control));
                    DrawSoundBankList(control, container);   
                });

                item.AddLabel("Wwise Event");
                ListItemTextInput eventInput = item.AddTextField(kvp.Value);
                eventInput.AddToClassList("soundbank-input");
                eventInput.eventManager.AddListener<ListItemInputChange>(e =>{
                    control.structure.soundBank[kvp.Key] = eventInput.value;
                    control.eventManager.Raise<StructureControlUpdated<SoundBank>>(new StructureControlUpdated<SoundBank>(control));
                });

                ListItemImage delButton = item.AddImage(delIcon);
                delButton.AddToClassList("icon","selectable","hoverable");
                delButton.eventManager.AddListener<MouseClickEvent>(e =>{
                    if(e.button != 0) return;
                    control.structure.soundBank.Remove(kvp.Key);
                    control.eventManager.Raise<StructureControlUpdated<SoundBank>>(new StructureControlUpdated<SoundBank>(control));
                    DrawSoundBankList(control, container);
                });

                container.Add(item);
                alternate = !alternate;
            }
        }
    }

}