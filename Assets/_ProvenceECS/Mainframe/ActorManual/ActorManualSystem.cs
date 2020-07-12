using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System;

namespace ProvenceECS.Mainframe{

    public class SetEntityToManualEntry : ProvenceEventArgs{
         public Entity entity;
        public ComponentHandle<ActorManualInstance> instanceHandle;
        public SetEntityToManualEntry(Entity entity){
            this.entity = entity;
            this.instanceHandle = null;
        }

        public SetEntityToManualEntry(Entity entity, ComponentHandle<ActorManualInstance> instanceHandle){
            this.entity = entity;
            this.instanceHandle = instanceHandle;
        }
    }

    public class ActorManualInstance : ProvenceComponent{
        public string key;

        public ActorManualInstance(){
            this.key = "";
        }

        public ActorManualInstance(string key){
            this.key = key;
        }
    }

    public class ActorManualSystem : ProvenceSystem{

        public ActorManualSystem(){}
        
        public override void Initialize(WorldRegistrationComplete args){
            //world.eventManager.AddListener<EditorPersistanceUpdateEvent>(EditorPersistanceUpdate);
            world.eventManager.AddListener<SetEntityToManualEntry>(SetEntityToManualEntry);
            world.eventManager.AddListener<ComponentAdded<ActorManualInstance>>(InstanceAdded);
            world.eventManager.AddListener<WakeSystemEvent>(SetInitialInstances);
            world.eventManager.Raise<SystemReadyEvent>(new SystemReadyEvent(this));
        }

        protected void SetInitialInstances(WakeSystemEvent args){
            foreach(KeyValuePair<Entity,ComponentHandle<ActorManualInstance>> kvp in world.componentManager.GetAllComponentsAsDictionary<ActorManualInstance>()){
                SetEntityToManualEntry(new SetEntityToManualEntry(kvp.Key,kvp.Value));
            }
        }

        protected void InstanceAdded(ComponentAdded<ActorManualInstance> args){
            SetEntityToManualEntry(new SetEntityToManualEntry(args.handle.entity,args.handle));
        }

        protected void SetEntityToManualEntry(SetEntityToManualEntry args){
            EntityHandle entityHandle = world.LookUpEntity(args.entity);
            if(entityHandle != null){
                ComponentHandle<ActorManualInstance> handle = args.instanceHandle == null ? entityHandle.GetComponent<ActorManualInstance>() : args.instanceHandle;
                if(handle != null && ProvenceManager.ActorManual.ContainsKey(handle.component.key)){
                    ActorManualEntry entry = ProvenceManager.ActorManual[handle.component.key];
                    entityHandle.AddComponent(entry.actorComponent.Clone());
                    foreach(dynamic component in entry.components.Values){
                        entityHandle.AddComponent(component.Clone());
                    }
                }    
            }      
        }

        public override void GatherCache(){
            throw new System.NotImplementedException();
        }

        public override void IntegrityCheck(CacheIntegrityChange args){
            //if(args.type == typeof(ActorManualInstance)) cacheIsSafe = false;
        }
    }

}