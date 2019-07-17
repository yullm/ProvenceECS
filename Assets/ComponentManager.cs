using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentDictionary : SerializableDictionary<System.Type,SerializableDictionary<Entity,ProvenceComponent>>{}

    public class ComponentManager : MonoBehaviour{

        public World world;
        public ComponentDictionary componentDictionary = new ComponentDictionary();

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            if(componentDictionary[typeof(T)] == null){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,ProvenceComponent>();
            }
            T component = entityHandle.gameObject.AddComponent<T>();
            componentDictionary[typeof(T)][entityHandle.entity] = component;
            return new ComponentHandle<T>(){entity = entityHandle.entity, component = component, world = world};
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            if(componentDictionary[typeof(T)] != null) {
                T component = componentDictionary[typeof(T)][entityHandle.entity] as T;
                if(component != null) Destroy(component);
                componentDictionary[typeof(T)].Remove(entityHandle.entity);
            }
        }

        private void RemoveComponent(EntityHandle entityHandle, System.Type type){
            if(componentDictionary[type] != null) {
                ProvenceComponent component = componentDictionary[type][entityHandle.entity];
                if(component != null) Destroy(component);
                componentDictionary[type].Remove(entityHandle.entity);
            }
        }

        public void RemoveEntityEntries(EntityHandle entityHandle){
            foreach(KeyValuePair<System.Type,SerializableDictionary<Entity,ProvenceComponent>> kvp in componentDictionary){
                if(kvp.Value[entityHandle.entity] != null){
                    RemoveComponent(entityHandle,kvp.Value.GetType());
                }
            }
        }
        
    }
}