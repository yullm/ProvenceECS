using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;
using ProvenceECS.Mainframe;
using UnityEngine;
using System.Threading.Tasks;
using Sjena.Selection;
using Sjena.Interact;

namespace ProvenceECS{

    public class ComponentAddedEarly<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentAddedEarly(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }

    public class ComponentAddedLate<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentAddedLate(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }

    public class ComponentRemovedEarly<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentRemovedEarly(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }

    public class ComponentRemovedLate<T> : ProvenceEventArgs where T : ProvenceComponent{
        public ComponentHandle<T> handle;

        public ComponentRemovedLate(ComponentHandle<T> handle){
            this.handle = handle;
        }
    }
    
    public class ComponentManager{

        public World world;
        [JsonProperty] protected Dictionary<Type,Dictionary<Entity,ProvenceComponent>> componentDictionary;
        protected Dictionary<Type,Dictionary<Entity,ProvenceComponent>> initialCopy;

        public ComponentManager(World world){
            this.world = world;
            this.componentDictionary = new Dictionary<Type, Dictionary<Entity, ProvenceComponent>>();
        }

        public void Initialize(){
            initialCopy = new Dictionary<Type, Dictionary<Entity, ProvenceComponent>>(componentDictionary);
            world.eventManager.AddListener<WakeSystemEvent>(AddInitial,-1);
        }

        public void Destroy(){
            componentDictionary.Clear();
            initialCopy.Clear();
        }

        protected void AddInitial(WakeSystemEvent args){
            foreach(Dictionary<Entity,ProvenceComponent> dict in initialCopy.Values.ToList()){
                foreach(KeyValuePair<Entity,ProvenceComponent> kvp in dict.OrderBy(entry => entry.Value.sortingIndex)){
                    AddInitialComponent(kvp.Key,(dynamic) kvp.Value);
                }
            }
        }

        protected void AddInitialComponent<T>(Entity entity, T component) where T : ProvenceComponent{
            ComponentHandle<T> handle = new ComponentHandle<T>(entity,component,world);
            new ComponentAddedEarly<T>(handle).Raise(world);
            new CacheUpdate<T>(world, GetAllComponentsAsDictionary<T>()).Raise(world);
            new ComponentAddedLate<T>(handle).Raise(world);
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity) where T : ProvenceComponent, new(){
            /* if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new Dictionary<Entity,ProvenceComponent>(); */
            T component = new T();
            return AddComponent<T>(entity,component);
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity, T component) where T : ProvenceComponent{
            if(component == null || entity == null) return null;
            Type componentType = component.GetType();            
            if(!componentDictionary.ContainsKey(componentType)){
                componentDictionary[componentType] = new Dictionary<Entity,ProvenceComponent>();
                world.systemManager.AddRequiredSystems(component.requiredSystems);
            }
            T exisitingComponent = componentDictionary[componentType].ContainsKey(entity) ? componentDictionary[componentType][entity] as T : null;
            ComponentHandle<T> handle;
            if(exisitingComponent != null && (exisitingComponent.alwaysPreventOverride || exisitingComponent.preventOverride)){
                handle = new ComponentHandle<T>(entity, exisitingComponent, world);
                if(exisitingComponent.Merge(component)){  
                    new ComponentAddedEarly<T>(handle).Raise(world);
                    new CacheUpdate<T>(world, GetAllComponentsAsDictionary<T>()).Raise(world);
                    new ComponentAddedLate<T>(handle).Raise(world);
                }                
                return handle;
            }
            componentDictionary[componentType][entity] = component as T;
            handle = new ComponentHandle<T>(entity, component, world);
            new ComponentAddedEarly<T>(handle).Raise(world);
            new CacheUpdate<T>(world, GetAllComponentsAsDictionary<T>()).Raise(world);
            new ComponentAddedLate<T>(handle).Raise(world);
            return handle;
        }

        public void AddComponentSet(Entity entity, HashSet<ProvenceComponent> set){
            foreach(dynamic component in set.OrderBy(c => c.sortingIndex)){
                AddComponent(entity, component);
            }
        }

        public void RemoveComponent<T>(Entity entity) where T : ProvenceComponent{
            try{
                if(entity == null) return;
                if(componentDictionary.ContainsKey(typeof(T))) {
                    if(componentDictionary[typeof(T)].ContainsKey(entity)){
                        ComponentHandle<T> handle = new ComponentHandle<T>(entity,componentDictionary[typeof(T)][entity] as T,world);
                        componentDictionary[typeof(T)].Remove(entity);
                        if(componentDictionary[typeof(T)].Count == 0) componentDictionary.Remove(typeof(T));
                        new ComponentRemovedEarly<T>(handle).Raise(world);
                        new CacheUpdate<T>(world, GetAllComponentsAsDictionary<T>()).Raise(world);
                        new ComponentRemovedLate<T>(handle).Raise(world);
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        protected void RemoveComponent<T>(Entity entity, T component) where T : ProvenceComponent{
            RemoveComponent<T>(entity);
        }

        public void RemoveAllComponents(Entity entity, params Type[] exceptions){
            HashSet<Type> exceptionTypes = new HashSet<Type>(exceptions);
            exceptionTypes.Add(typeof(Name));
            foreach(ComponentHandle<ProvenceComponent> handle in GetAllComponents(entity)){
                dynamic component = handle.component;
                if(!exceptionTypes.Contains(component.GetType()))
                    RemoveComponent(entity, component);
            }
        }

        public void RemoveEntityEntries(Entity entity){
            if(entity == null) return;
            List<Type> types = componentDictionary.Keys.ToList();
            foreach(ComponentHandle<ProvenceComponent> handle in GetAllComponents(entity)){
                dynamic component = handle.component;
                RemoveComponent(entity, component);
            }
        }

        public ComponentHandle<T> GetOrCreateComponent<T>(Entity entity) where T : ProvenceComponent, new(){
            ComponentHandle<T> handle = GetComponent<T>(entity);
            if(handle == null) handle = AddComponent<T>(entity);
            return handle;
        }

        public ComponentHandle<T> GetComponent<T>(Entity entity) where T : ProvenceComponent{
            if(entity == null) return null;
            if(componentDictionary.ContainsKey(typeof(T))){
                if(!componentDictionary[typeof(T)].ContainsKey(entity)) return null;
                T component = componentDictionary[typeof(T)][entity] as T;
                return new ComponentHandle<T>(entity, component, world);
            }
            return null;
        }

        public ComponentHandle<ProvenceComponent> GetComponent(Entity entity, Type type){
            if(entity == null) return null;
            if(componentDictionary.ContainsKey(type)){
                if(!componentDictionary[type].ContainsKey(entity)) return null;
                ProvenceComponent component = componentDictionary[type][entity];
                return new ComponentHandle<ProvenceComponent>(entity, component, world);
            }
            return null;
        }

        public bool TryGetComponent<T>(Entity entity, out ComponentHandle<T> componentHandle) where T : ProvenceComponent{
            componentHandle = GetComponent<T>(entity);
            return componentHandle != null;
        }

        public HashSet<ComponentHandle<ProvenceComponent>> GetAllComponents(Entity entity){
            HashSet<ComponentHandle<ProvenceComponent>> handles = new HashSet<ComponentHandle<ProvenceComponent>>();
            if(entity == null) return handles;
            foreach(KeyValuePair<Type,Dictionary<Entity,ProvenceComponent>> kvp in componentDictionary){
                if(kvp.Value.ContainsKey(entity))
                    handles.Add(new ComponentHandle<ProvenceComponent>(entity,kvp.Value[entity],world));
            }
            return handles; 
        }

        public HashSet<ComponentHandle<T>> GetAllComponents<T>() where T : ProvenceComponent{
            HashSet<ComponentHandle<T>> handles = new HashSet<ComponentHandle<T>>();
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

        public HashSet<ComponentHandle<T>> GetAllComponentsByBase<T>() where T : ProvenceComponent{
            HashSet<ComponentHandle<T>> children = new HashSet<ComponentHandle<T>>();
            foreach(Type keyType in componentDictionary.Keys){
                if(keyType.IsAssignableFrom(typeof(T))){
                    foreach(KeyValuePair<Entity,ProvenceComponent> kvp in componentDictionary[typeof(T)]){
                        ComponentHandle<T> componentHandle = new ComponentHandle<T>(kvp.Key, kvp.Value as T, world);
                        children.Add(componentHandle);
                    }
                }
            }
            return children;
        }

        /* public Dictionary<Entity,ProvenceComponent> GetEntry<T>() where T : ProvenceComponent{
            if(componentDictionary.ContainsKey(typeof(T))) return componentDictionary[typeof(T)];
            return new Dictionary<Entity, ProvenceComponent>();
        } */
        
    }
}