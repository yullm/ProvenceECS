using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentDictionary : SerializableDictionary<System.Type,SerializableDictionary<Entity,ProvenceComponent>>{}

    public class ComponentManager : MonoBehaviour{

        public World world;

        public ComponentDictionary componentDictionary = new ComponentDictionary();

        public T AddComponent<T>(Entity e) where T : ProvenceComponent{
            if(componentDictionary[typeof(T)] == null){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,ProvenceComponent>();
            }
            T component = world.LookUpEntity(e).AddComponent<T>();
            componentDictionary[typeof(T)][e] = component;
            return component;
        }

        public void RemoveComponent<T>(Entity e) where T : ProvenceComponent{
            if(componentDictionary[typeof(T)] != null) {
                T component = componentDictionary[typeof(T)][e] as T;
                if(component != null) Destroy(component);
                componentDictionary[typeof(T)].Remove(e);
            }
        }

        public void RemoveEntityEntries(Entity e){
            foreach(KeyValuePair<System.Type,SerializableDictionary<Entity,ProvenceComponent>> kvp in componentDictionary){
                //if(kvp.Value[e] != null)
                //Get it to call RemoveComponent 
            }
        }
        
}
}