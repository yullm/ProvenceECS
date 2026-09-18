using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceComponent{
        
        [DontDisplayInEditor] public string id;
        [DontDisplayInEditor] [JsonIgnore] public byte sortingIndex;
        [DontDisplayInEditor] [JsonIgnore] public HashSet<System.Type> requiredSystems;
        [DontDisplayInEditor] public bool preventOverride;
        [DontDisplayInEditor] public bool alwaysPreventOverride;

        public ProvenceComponent(){
            id = System.Guid.NewGuid().ToString();
            sortingIndex = 0;
            requiredSystems = new HashSet<System.Type>();
            preventOverride = false;
            alwaysPreventOverride = false;
        }

        public ProvenceComponent(string id) : this(){
            this.id = id;
        }

        public virtual ProvenceComponent Clone(){
            ProvenceComponent clone = this.MemberwiseClone() as ProvenceComponent;
            clone.id = System.Guid.NewGuid().ToString();
            return clone;
        }

        public virtual bool Merge(ProvenceComponent otherComponent){
            return true;
        }

    }

    public class ComponentHandle<T> where T : ProvenceComponent{
        
        public Entity entity;
        public T component;
        public World world;

        public ComponentHandle(){
            this.entity = null;
            this.component = null;
            this.world = null;
        }

        public ComponentHandle(Entity entity, T component, World world){
            this.entity = entity;
            this.component = component;
            this.world = world;
        }

        public void Destroy(){
            world.LookUpEntity(entity).RemoveComponent<T>();
        }

    }

}