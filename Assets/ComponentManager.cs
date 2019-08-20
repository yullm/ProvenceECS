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
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            T component = entityHandle.gameObject.AddComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component as T;
            if(paramList.Length > 0){
                FieldInfo[] fields = typeof(T).GetFields();
                for(int i = 0; i < paramList.Length; i++){
                    if(i >= fields.Length) break;
                    try{ fields[i].SetValue(component, paramList[i]); }
                    catch(System.Exception e){ print(e); }
                } 
            }
            return new ComponentHandle<T>(entityHandle.entity, component, world);
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle) where T : Component{
            if(!componentDictionary.ContainsKey(typeof(T)))
                componentDictionary[typeof(T)] = new SerializableDictionary<Entity,Component>();
            T component = entityHandle.gameObject.GetComponent<T>() as T;
            componentDictionary[typeof(T)][entityHandle.entity] = component;
            return new ComponentHandle<T>(entityHandle.entity, component, world);
        }

        public void RegisterAllComponents(EntityHandle entityHandle){
            Component[] components = entityHandle.gameObject.GetComponents<Component>();
            foreach(Component component in components) {
                System.Type type = component.GetType();
                if(type != typeof(Entity)){
                    var method = typeof(ComponentManager).GetMethod("RegisterComponent");
                    var reference = method.MakeGenericMethod(type);
                    reference.Invoke(this,new object[]{entityHandle});
                }
            }
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