using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System;

namespace ProvenceECS.Mainframe{

    public class SubActorEntry{
        public string key;
        public Vector3 position;
        public Entity subEntity;

        public SubActorEntry(){
            key = "";
            position = new();
            subEntity = null;
        }
    }

    public class SubActors : ProvenceComponent{
        public List<SubActorEntry> subActors;

        public SubActors(){
            subActors = new();
        }
    }

    public class ActorManualSystem : ProvenceSystem{

        public ActorManualSystem(){}
        
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<SetEntityToManualEntry<ActorManualEntry>>(SetEntityToManualEntry);
            world.eventManager.AddListener<CreateEntryInstance<ActorManualEntry>>(CreateEntryInstance);
            if(Application.isPlaying) world.eventManager.AddListener<WakeSystemEvent>(SetInitialInstances);
            world.eventManager.AddListener<ComponentAdded<SubActors>>(SubActorAdded);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<SetEntityToManualEntry<ActorManualEntry>>(SetEntityToManualEntry);
            world.eventManager.RemoveListener<CreateEntryInstance<ActorManualEntry>>(CreateEntryInstance);
            world.eventManager.RemoveListener<WakeSystemEvent>(SetInitialInstances);
            world.eventManager.RemoveListener<ComponentAdded<SubActors>>(SubActorAdded);
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

#region SubActor

        protected void SubActorAdded(ComponentAdded<SubActors> args){
            Entity parentEntity = args.handle.entity;
            List<SubActorEntry> subActors = args.handle.component.subActors;

            GameObject go = world.GetComponent<UnityGameObject>(parentEntity)?.component.gameObject;
            if(go == null) return;  
            foreach(SubActorEntry entry in subActors){
                if(entry.key == "") continue;
            
                EntityHandle subHandle = world.LookUpEntity(entry.subEntity) ?? world.CreateEntity();
                Vector3 pos = go.transform.position + (go.transform.rotation * entry.position);
                SetEntityToManualEntry(new SetEntityToManualEntry<ActorManualEntry>(subHandle.entity,entry.key,pos));
            }
        }

#endregion

    }

}