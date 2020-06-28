using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceComponent{
        [DontDisplayInEditor]
        public string id;

        public ProvenceComponent(){
            this.id = System.Guid.NewGuid().ToString();
        }

        public ProvenceComponent(string id){
            this.id = id;
        }

        public ProvenceComponent Clone(){
            ProvenceComponent clone = this.MemberwiseClone() as ProvenceComponent;
            clone.id = System.Guid.NewGuid().ToString();
            return clone;
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