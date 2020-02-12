using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProvenceECS{

    public class ComponentDictionary : Dictionary<System.Type,Dictionary<Entity,List<Component>>>{}

    public class ComponentManager : MonoBehaviour{

        public World world;
        private ComponentDictionary componentDictionary = new ComponentDictionary();

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, params object[] paramList) where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new Dictionary<Entity,List<Component>>();
            if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity))
                componentDictionary[typeof(T)][entityHandle.entity] = new List<Component>();
            T component = entityHandle.gameObject.AddComponent<T>() as T;            
            componentDictionary[typeof(T)][entityHandle.entity].Add(component as T);
            if(paramList.Length > 0){
                FieldInfo[] fields = typeof(T).GetFields();
                for(int i = 0; i < paramList.Length; i++){
                    if(i >= fields.Length) break;
                    try{ fields[i].SetValue(component, paramList[i]); }
                    catch(System.Exception e){ print(e); }
                } 
            }
            world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,typeof(T)));
            return new ComponentHandle<T>(entityHandle.entity, component, world);
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle) where T : Component{
            
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new Dictionary<Entity,List<Component>>();
            if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity))
                componentDictionary[typeof(T)][entityHandle.entity] = new List<Component>();

            T component = entityHandle.gameObject.GetComponentInChildren<T>() as T;
            if(!componentDictionary[typeof(T)][entityHandle.entity].Contains(component))
                componentDictionary[typeof(T)][entityHandle.entity].Add(component as T);

            if(typeof(T).IsSubclassOf(typeof(ComponentSystem))){
                ComponentSystem componentSystem = component as ComponentSystem;
                componentSystem.world = world;
                componentSystem.entity = entityHandle.entity;
            }
            
            return new ComponentHandle<T>(entityHandle.entity, component as T, world);
        }

        public void RegisterComponents<T>(EntityHandle entityHandle) where T : Component{
            
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new Dictionary<Entity,List<Component>>();
            if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)){
                componentDictionary[typeof(T)][entityHandle.entity] = new List<Component>();

                foreach(T component in entityHandle.gameObject.GetComponentsInChildren<T>()){
                     if(component.gameObject.tag.Equals("EntityIgnore")) continue;
                    if(componentDictionary[typeof(T)][entityHandle.entity].Contains(component)) continue;
                    componentDictionary[typeof(T)][entityHandle.entity].Add(component as T);

                    if(typeof(T).IsSubclassOf(typeof(ComponentSystem))){
                        ComponentSystem componentSystem = component as ComponentSystem;
                        componentSystem.world = world;
                        componentSystem.entity = entityHandle.entity;
                    }
                }
            }
        }

        public void RegisterAllComponents(EntityHandle entityHandle){
            List<Component> components = new List<Component>(entityHandle.gameObject.GetComponentsInChildren<Component>());
            int entityCount = 0;
            foreach(Component component in components){
                System.Type type = component.GetType();
                if(type != typeof(Entity)){
                    var method = typeof(ComponentManager).GetMethod("RegisterComponents");
                    var reference = method.MakeGenericMethod(type);
                    reference.Invoke(this,new object[]{entityHandle});
                }else{
                    entityCount++;
                    if(entityCount > 1 ) return;
                }
            }
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : Component{
            try{
                if(componentDictionary.ContainsKey(typeof(T))) {
                    if(componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)){
                        if(componentDictionary[typeof(T)][entityHandle.entity].Count == 0){ 
                            componentDictionary[typeof(T)].Remove(entityHandle.entity);
                            return;
                        }
                        int latestIndex = componentDictionary[typeof(T)][entityHandle.entity].Count - 1;
                        T component = componentDictionary[typeof(T)][entityHandle.entity][latestIndex] as T;
                        componentDictionary[typeof(T)][entityHandle.entity].Remove(component);
                        Destroy(component);
                        if(componentDictionary[typeof(T)].Count == 0) componentDictionary[typeof(T)].Remove(entityHandle.entity);
                        world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,typeof(T)));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        public void RemoveComponent(EntityHandle entityHandle, System.Type type){
            try{
                if(componentDictionary.ContainsKey(type)) {
                    if(componentDictionary[type].ContainsKey(entityHandle.entity)){
                        if(componentDictionary[type][entityHandle.entity].Count == 0){ 
                            componentDictionary[type].Remove(entityHandle.entity);
                            return;
                        }
                        int latestIndex = componentDictionary[type][entityHandle.entity].Count - 1;
                        Component component = componentDictionary[type][entityHandle.entity][latestIndex];
                        componentDictionary[type][entityHandle.entity].Remove(component);
                        Destroy(component);
                        if(componentDictionary[type].Count == 0) componentDictionary[type].Remove(entityHandle.entity);
                        world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,type));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }        

        public void RemoveComponent<T>(EntityHandle entityHandle, T component) where T : Component{
            try{
                if(componentDictionary.ContainsKey(typeof(T))) {
                    if(componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)){
                        if(componentDictionary[typeof(T)][entityHandle.entity].Count == 0){ 
                            componentDictionary[typeof(T)].Remove(entityHandle.entity);
                            return;
                        }
                        if(!componentDictionary[typeof(T)][entityHandle.entity].Contains(component)) return;
                        componentDictionary[typeof(T)][entityHandle.entity].Remove(component);
                        Destroy(component);
                        if(componentDictionary[typeof(T)].Count == 0) componentDictionary[typeof(T)].Remove(entityHandle.entity);
                        world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,typeof(T)));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        public void RemoveComponents<T>(EntityHandle entityHandle) where T : Component{
            try{
                if(componentDictionary.ContainsKey(typeof(T))) {
                    if(componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)){
                        if(componentDictionary[typeof(T)][entityHandle.entity].Count == 0){ 
                            componentDictionary[typeof(T)].Remove(entityHandle.entity);
                            return;
                        }
                        for(int i = 0; i < componentDictionary[typeof(T)][entityHandle.entity].Count; i++){
                            T component = componentDictionary[typeof(T)][entityHandle.entity][i] as T;
                            Destroy(component);   
                        }
                        componentDictionary[typeof(T)].Remove(entityHandle.entity);
                        world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,typeof(T)));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        public void RemoveComponents(EntityHandle entityHandle, System.Type type){
            try{
                if(componentDictionary.ContainsKey(type)) {
                    if(componentDictionary[type].ContainsKey(entityHandle.entity)){
                        foreach(Component component in componentDictionary[type][entityHandle.entity]){
                            Destroy(component);
                        }
                        componentDictionary[type].Remove(entityHandle.entity);
                        world.eventManager.Raise<CacheIntegrityChange>(CacheIntegrityChange.CreateInstance(world,type));
                    }
                }
            }catch(System.Exception e){
                Debug.Log("Improper reference to component: " + e);
            }
        }

        public void RemoveEntityEntries(EntityHandle entityHandle){
            foreach(KeyValuePair<System.Type,Dictionary<Entity,List<Component>>> kvp in componentDictionary){
                if(kvp.Value[entityHandle.entity] != null){
                    RemoveComponents(entityHandle,kvp.Key);
                }
            }
            foreach(var comp in entityHandle.gameObject.GetComponents<Component>()){
                if(!(comp is Transform)) Destroy(comp);
            }
        }

        public ComponentHandle<T> GetComponentByEntity<T>(EntityHandle entityHandle) where T : Component{
            if(componentDictionary.ContainsKey(typeof(T))){
                if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)) return null;
                if(componentDictionary[typeof(T)][entityHandle.entity].Count <= 0){
                    componentDictionary[typeof(T)].Remove(entityHandle.entity);
                    return null;
                }
                int latestIndex = componentDictionary[typeof(T)][entityHandle.entity].Count - 1;
                T component = componentDictionary[typeof(T)][entityHandle.entity][latestIndex] as T;
                return new ComponentHandle<T>(entityHandle.entity, component, world);
            }
            return null;
        }

        public ComponentHandle<Component> GetComponentByEntity(EntityHandle entityHandle, System.Type type){
            if(componentDictionary.ContainsKey(type)){
                if(!componentDictionary[type].ContainsKey(entityHandle.entity)) return null;
                if(componentDictionary[type][entityHandle.entity].Count <= 0){
                    componentDictionary[type].Remove(entityHandle.entity);
                    return null;
                }
                int latestIndex = componentDictionary[type][entityHandle.entity].Count - 1;
                Component component = componentDictionary[type][entityHandle.entity][latestIndex];
                return new ComponentHandle<Component>(entityHandle.entity, component, world);
            }
            return null;
        }

        public List<ComponentHandle<T>> GetAllComponentsByEntity<T>(EntityHandle entityHandle) where T : Component{
            List<ComponentHandle<T>> list = new List<ComponentHandle<T>>();
            if(componentDictionary.ContainsKey(typeof(T))){
                if(!componentDictionary[typeof(T)].ContainsKey(entityHandle.entity)) return list;
                foreach(T component in componentDictionary[typeof(T)][entityHandle.entity]){
                    list.Add(new ComponentHandle<T>(entityHandle.entity, component, world));
                } 
            }
            return list;
        }

        public List<ComponentHandle<Component>> GetAllComponentsByEntity(EntityHandle entityHandle, System.Type type){
            List<ComponentHandle<Component>> list = new List<ComponentHandle<Component>>();
            if(componentDictionary.ContainsKey(type)){
                if(!componentDictionary[type].ContainsKey(entityHandle.entity)) return list;
                foreach(Component component in componentDictionary[type][entityHandle.entity]){
                    list.Add(new ComponentHandle<Component>(entityHandle.entity, component, world));
                } 
            }
            return list;
        }

        public List<ComponentHandle<T>> GetAllComponentsOfType<T>() where T : Component{
            List<ComponentHandle<T>> handles = new List<ComponentHandle<T>>();
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,List<Component>> kvp in componentDictionary[typeof(T)]){
                    foreach(Component component in kvp.Value){
                        handles.Add(new ComponentHandle<T>(kvp.Key,component as T, world));
                    }   
                }
            }
            return handles;
        }

        public Dictionary<System.Type,List<ComponentHandle<T>>> GetAllComponentsByInheritance<T>() where T : Component{
            Dictionary<System.Type,List<ComponentHandle<T>>> children = new Dictionary<System.Type, List<ComponentHandle<T>>>();
            foreach(System.Type keyType in componentDictionary.Keys){
                if(keyType.IsSubclassOf(typeof(T))){
                    foreach(KeyValuePair<Entity,List<Component>> kvp in componentDictionary[typeof(T)]){
                        foreach(Component component in kvp.Value){
                            ComponentHandle<T> componentHandle = new ComponentHandle<T>(kvp.Key, component as T, world);
                            if(children[keyType] == null) children[keyType] = new List<ComponentHandle<T>>();
                            children[keyType].Add(componentHandle);
                        } 
                    }
                }
            }
            return children;
        }

        public EntityHandle GetEntityByComponent(Component component){
            System.Type componentType = component.GetType();
            if(componentDictionary.ContainsKey(componentType)){

                foreach(KeyValuePair<Entity,List<Component>> kvp in componentDictionary[componentType]){
                    foreach(Component currentComponent in kvp.Value){
                        if(currentComponent == component) return world.LookUpEntity(kvp.Key);
                    }                    
                }

            }            
            return null;
        }

        public EntityHandle GetEntityByComponent<T>(T component) where T : Component{
            if(componentDictionary.ContainsKey(typeof(T))){
                foreach(KeyValuePair<Entity,List<Component>> kvp in componentDictionary[typeof(T)]){
                    foreach(T currentComponent in kvp.Value){
                        if(currentComponent == component) return world.LookUpEntity(kvp.Key);
                    }                    
                }

            }            
            return null;
        }

        public bool ValidateConditionalComponentList(Entity entity, List<StringConditional> list){
           foreach(StringConditional current in list){
               if(current.conditionName.Equals("")) continue;
                bool found = false;
                foreach(KeyValuePair<System.Type,Dictionary<Entity,List<Component>>> kvp in componentDictionary){
                    if(kvp.Key.Name.Equals(current.conditionName)){
                        found = true;
                        if(kvp.Value.ContainsKey(entity) != current.conditionState) return false;
                    }
                }
                if(!found && current.conditionState){ 
                    print("Couldn't find Component type: " + current.conditionName + ", but was required");
                    return false;
                }
            } 
            return true;
        }

        public bool EntityHasComponent<T>(Entity entity) where T : Component{
            foreach(KeyValuePair<System.Type,Dictionary<Entity,List<Component>>> kvp in componentDictionary){
                if(kvp.Key == typeof(T)){
                    return kvp.Value.ContainsKey(entity);
                }
            }
            return false;
        }

        public bool EntityHasComponent(Entity entity, System.Type type){
            foreach(KeyValuePair<System.Type,Dictionary<Entity,List<Component>>> kvp in componentDictionary){
                if(kvp.Key == type){
                    return kvp.Value.ContainsKey(entity);
                }
            }
            return false;
        }
        

        public bool EntityHasComponent(Entity entity, string componentName){
            foreach(KeyValuePair<System.Type,Dictionary<Entity,List<Component>>> kvp in componentDictionary){
                if(kvp.Key.Name.Equals(componentName)){
                    return kvp.Value.ContainsKey(entity);
                }
            }
            return false;
        }
        
    }
}