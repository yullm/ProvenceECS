using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentDictionary : SerializableDictionary<System.Type,SerializableDictionary<Entity,Component>>{}

    public class ComponentManager : MonoBehaviour{

        public World world;
        public ComponentDictionary componentDictionary = new ComponentDictionary();

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, params object[] paramList) where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T))){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            }
            T component = entityHandle.gameObject.AddComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component as T;
            if(paramList.Length > 0){
                print(typeof(T));
                FieldInfo[] fields = typeof(T).GetFields();
                for(int i = 0; i < fields.Length; i++){
                    //if(i >= fields.Length) break;
                    print(fields[i]);
                    //fields[i].SetValue(component, paramList[i]);
                }
                
            }
            return new ComponentHandle<T>(entityHandle.entity, component, world);
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle) where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T))){
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            }
            T component = entityHandle.gameObject.GetComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component;
            return new ComponentHandle<T>(entityHandle.entity, component, world);
        }

        private void RegisterComponent(EntityHandle entityHandle, System.Type type){
            if(!componentDictionary.ContainsKey(type)){
                componentDictionary[type] = new SerializableDictionary<Entity,Component>();
            }
            Component component = entityHandle.gameObject.GetComponent(type);
            componentDictionary[type][entityHandle.entity] = component;
        }

        public void RegisterAllComponents(EntityHandle entityHandle){
            Component[] components = entityHandle.gameObject.GetComponents<Component>();
            foreach(Component component in components) 
                if(component.GetType() != typeof(Entity)) RegisterComponent(entityHandle,component.GetType());
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

        public List<ComponentHandle<T>> GetAllComponentsOfType<T>() where T : Component{
            List<ComponentHandle<T>> handles = new List<ComponentHandle<T>>();
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,Component> kvp in componentDictionary[typeof(T)]){
                    handles.Add(new ComponentHandle<T>(kvp.Key,kvp.Value as T, world));
                }
            }
            return handles;
        }
        
    }
}