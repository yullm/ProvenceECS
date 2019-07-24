using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentDictionary : SerializableDictionary<System.Type,SerializableDictionary<Entity,Component>>{}

    public class ComponentManager : MonoBehaviour{

        public World world;
        public ComponentDictionary componentDictionary = new ComponentDictionary();

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle) where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T))){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            }
            T component = entityHandle.gameObject.AddComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component as T;
            return new ComponentHandle<T>(){entity = entityHandle.entity, component = component, world = world};
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle)where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T))){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            }
            T component = entityHandle.gameObject.GetComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component;
            return new ComponentHandle<T>(){entity = entityHandle.entity, component = component, world = world};
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : Component{
            if(componentDictionary.ContainsKey(typeof(T))) {
                T component = componentDictionary[typeof(T)][entityHandle.entity] as T;
                if(component != null) Destroy(component);
                componentDictionary[typeof(T)].Remove(entityHandle.entity);
            }
        }

        private void RemoveComponent(EntityHandle entityHandle, System.Type type){
            if(componentDictionary[type] != null) {
                Component component = componentDictionary[type][entityHandle.entity];
                if(component != null) Destroy(component);
                componentDictionary[type].Remove(entityHandle.entity);
            }
        }

        public void RemoveEntityEntries(EntityHandle entityHandle){
            foreach(KeyValuePair<System.Type,SerializableDictionary<Entity,Component>> kvp in componentDictionary){
                if(kvp.Value[entityHandle.entity] != null){
                    RemoveComponent(entityHandle,kvp.Key);
                }
            }
            foreach(var comp in entityHandle.gameObject.GetComponents<Component>()){
                if(!(comp is Transform)) Destroy(comp);
            }
        }

        public void RemoveEntityEntriesPermanently(Entity entity){
            foreach(KeyValuePair<System.Type,SerializableDictionary<Entity,Component>> kvp in componentDictionary){
                if(kvp.Value[entity] != null){
                    kvp.Value.Remove(entity);
                }
            }
        }
        
    }
}