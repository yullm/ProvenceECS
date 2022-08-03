using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System;

namespace ProvenceECS.Mainframe{

    public class ActorManualSystem : ProvenceSystem{

        public ActorManualSystem(){}
        
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<SetEntityToManualEntry<ActorManualEntry>>(SetEntityToManualEntry);
            world.eventManager.AddListener<CreateEntryInstance<ActorManualEntry>>(CreateEntryInstance);
            if(Application.isPlaying) world.eventManager.AddListener<WakeSystemEvent>(SetInitialInstances);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<SetEntityToManualEntry<ActorManualEntry>>(SetEntityToManualEntry);
            world.eventManager.RemoveListener<CreateEntryInstance<ActorManualEntry>>(CreateEntryInstance);
            world.eventManager.RemoveListener<WakeSystemEvent>(SetInitialInstances);
        }

        protected void SetInitialInstances(WakeSystemEvent args){
            foreach(KeyValuePair<Entity,ComponentHandle<ProvenceCollectionInstance<ActorManualEntry>>> kvp in world.componentManager.GetAllComponentsAsDictionary<ProvenceCollectionInstance<ActorManualEntry>>()){
                SetEntityToManualEntry(new SetEntityToManualEntry<ActorManualEntry>(kvp.Key,kvp.Value.component.key));
            }
        }

        protected void SetEntityToManualEntry(SetEntityToManualEntry<ActorManualEntry> args){
            ProvenceManager.Collections<ActorManualEntry>().SetEntityToEntry(world, args.entity, args.key, args.position, args.rotation);    
        }

        protected void CreateEntryInstance(CreateEntryInstance<ActorManualEntry> args){
            ProvenceManager.Collections<ActorManualEntry>().CreateEntryInstance(world, args.key, args.position, args.rotation);
        }

    }

}