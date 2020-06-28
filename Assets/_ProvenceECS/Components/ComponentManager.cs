using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;
using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentAdded<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentAdded(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }

    public class ComponentRemoved<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentRemoved(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }
    
    public class ComponentManager{

        public World world;
        [JsonProperty] protected Dictionary<Type,Dictionary<Entity,ProvenceComponent>> componentDictionary;

        public ComponentManager(World world){
            this.world = world;
            this.componentDictionary = new Dictionary<Type, Dictionary<Entity, ProvenceComponent>>();
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent, new(){
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new Dictionary<Entity,ProvenceComponent>();
            T component = new T();
            return AddComponent<T>(entityHandle,component);
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, T component) where T : ProvenceComponent{
            if(component == null || entityHandle == null) return null;
            Type componentType = component.GetType();            
            if(!componentDictionary.ContainsKey(componentType))
                componentDictionary[componentType] = new Dictionary<Entity,ProvenceComponent>();
            componentDictionary[componentType][entityHandle.entity] = component as T;
            ComponentHandle<T> handle = new ComponentHandle<T>(entityHandle.entity, component, world);
            world.eventManager.Raise<CacheIntegrityChange>(new CacheIntegrityChange(componentType));
            world.eventManager.Raise<ComponentAdded<T>>(new ComponentAdded<T>(handle));
            return handle;
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            try{
                if(entityHandle == null) return;
                if(componentDictionary.ContainsKey(typeof(T))) {
                    if(componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)){
                        ComponentHandle<T> handle = new ComponentHandle<T>(entityHandle.entity,componentDictionary[typeof(T)][entityHandle.entity] as T,world);
                        componentDictionary[typeof(T)].Remove(entityHandle.entity);
                        if(componentDictionary[typeof(T)].Count == 0) componentDictionary.Remove(typeof(T));
                        world.eventManager.Raise<CacheIntegrityChange>(new CacheIntegrityChange(typeof(T)));
                        world.eventManager.Raise<ComponentRemoved<T>>(new ComponentRemoved<T>(handle));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        public void RemoveComponent<T>(EntityHandle entityHandle, T component) where T : ProvenceComponent{
            RemoveComponent<T>(entityHandle);
        }     

        public void RemoveEntityEntries(EntityHandle entityHandle){
            if(entityHandle == null) return;
            List<Type> types = componentDictionary.Keys.ToList();
            foreach(ComponentHandle<ProvenceComponent> handle in GetAllComponents(entityHandle)){
                dynamic component = handle.component;
                RemoveComponent(entityHandle, component);
            }
        }

        public ComponentHandle<T> GetOrCreateComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent, new(){
            ComponentHandle<T> handle = GetComponent<T>(entityHandle);
            if(handle == null) handle = AddComponent<T>(entityHandle);
            return handle;
        }

        public ComponentHandle<T> GetComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            if(entityHandle == null) return null;
            if(componentDictionary.ContainsKey(typeof(T))){
                if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)) return null;
                T component = componentDictionary[typeof(T)][entityHandle.entity] as T;
                return new ComponentHandle<T>(entityHandle.entity, component, world);
            }
            return null;
        }

        public ComponentHandle<ProvenceComponent> GetComponent(EntityHandle entityHandle, Type type){
            if(entityHandle == null) return null;
            if(componentDictionary.ContainsKey(type)){
                if(!componentDictionary[type].ContainsKey(entityHandle.entity)) return null;
                ProvenceComponent component = componentDictionary[type][entityHandle.entity];
                return new ComponentHandle<ProvenceComponent>(entityHandle.entity, component, world);
            }
            return null;
        }

        public List<ComponentHandle<ProvenceComponent>> GetAllComponents(EntityHandle entityHandle){
            List<ComponentHandle<ProvenceComponent>> handles = new List<ComponentHandle<ProvenceComponent>>();
            if(entityHandle == null) return handles;
            foreach(KeyValuePair<Type,Dictionary<Entity,ProvenceComponent>> kvp in componentDictionary){
                if(kvp.Value.ContainsKey(entityHandle.entity))
                    handles.Add(new ComponentHandle<ProvenceComponent>(entityHandle.entity,kvp.Value[entityHandle.entity],world));
            }
            return handles; 
        }

        public List<ComponentHandle<T>> GetAllComponents<T>() where T : ProvenceComponent{
            List<ComponentHandle<T>> handles = new List<ComponentHandle<T>>();
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[typeof(T)]){
                    handles.Add(new ComponentHandle<T>(kvp.Key, kvp.Value as T, world));
                }
            }
            return handles;
        }

        public Dictionary<Entity,ComponentHandle<T>> GetAllComponentsAsDictionary<T>() where T : ProvenceComponent{
            Dictionary<Entity,ComponentHandle<T>> dict = new Dictionary<Entity, ComponentHandle<T>>();
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[typeof(T)]){
                    dict[kvp.Key] = new ComponentHandle<T>(kvp.Key, kvp.Value as T, world);
                }
            }
            return dict;
        }

        public List<ComponentHandle<T>> GetAllComponentsByInheritance<T>() where T : ProvenceComponent{
            List<ComponentHandle<T>> children = new List<ComponentHandle<T>>();
            foreach(Type keyType in componentDictionary.Keys){
                if(keyType.IsSubclassOf(typeof(T))){
                    foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[typeof(T)]){
                        ComponentHandle<T> componentHandle = new ComponentHandle<T>(kvp.Key, kvp.Value as T, world);
                        children.Add(componentHandle);
                    }
                }
            }
            return children;
        }

        public EntityHandle GetEntityByComponent(ProvenceComponent component){
            Type componentType = component.GetType();
            if(componentDictionary.ContainsKey(componentType)){
                foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[componentType]){
                    if(kvp.Value == component) return world.LookUpEntity(kvp.Key);
                }
            }            
            return null;
        }

        public EntityHandle GetEntityByComponent<T>(T component) where T : ProvenceComponent{
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[typeof(T)]){
                    if(kvp.Value == component) return world.LookUpEntity(kvp.Key);
                }
            }            
            return null;
        }

        public void Organize(){
            componentDictionary = componentDictionary.OrderBy(d => d.Key.Name).ToDictionary(d => d.Key, d => d.Value);
        }
        
    }
}