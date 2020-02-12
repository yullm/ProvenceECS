using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentHandle<T> where T : Component{
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