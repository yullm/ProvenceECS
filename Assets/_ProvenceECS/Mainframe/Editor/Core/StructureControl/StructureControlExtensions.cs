using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ransacked;

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
    }

}