using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [RequireComponent(typeof(EntityManager),typeof(ComponentManager),typeof(EventManager))]
    [RequireComponent(typeof(SystemManager))]
    public class World : MonoBehaviour
    {
        public int id;
        public string worldName;
        public WorldManager manager;
        public EntityManager entityManager;
        public ComponentManager componentManager;
        public EventManager eventManager;
        public SystemManager systemManager;

        public void InitWorld(){
            if(entityManager == null) entityManager = GetComponent<EntityManager>();
            if(componentManager == null) componentManager = GetComponent<ComponentManager>();
            if(eventManager == null) eventManager = GetComponent<EventManager>();
            if(systemManager == null) systemManager = GetComponent<SystemManager>();
            if(entityManager.world == null) entityManager.world = this;
            if(componentManager.world == null) componentManager.world = this;
            if(eventManager.world == null) eventManager.world = this;
            if(systemManager.world == null) systemManager.world = this;  
        }

        void Update(){
            if(eventManager) eventManager.Raise<WorldUpdateEvent>(new WorldUpdateEvent(this,Time.time));
        }

        public EntityHandle CreateEntity(){
            return entityManager.CreateEntity();
        }

        public void RemoveEntity(EntityHandle entityHandle){
            entityManager.RemoveEntity(entityHandle);
            componentManager.RemoveEntityEntries(entityHandle);
        }

        public EntityHandle LookUpEntity(Entity entity){
            return entityManager.LookUpEntity(entity);
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, params object[] paramList) where T : Component{
            return componentManager.AddComponent<T>(entityHandle,paramList);
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle) where T : Component{
            return componentManager.RegisterComponent<T>(entityHandle);
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : Component{
            componentManager.RemoveComponent<T>(entityHandle);
        }

        public void AddSystem<T>() where T : ProvenceSystem{
            systemManager.AddSystem<T>();
        }

        public void RemoveSystem<T>() where T : ProvenceSystem{
            systemManager.RemoveSystem<T>();
        }

        public void RegisterInitialEntities(){
            Entity[] initialEntities = GetComponentsInChildren<Entity>();
            foreach(Entity entity in initialEntities){
                EntityHandle entityHandle = entityManager.RegisterEntity(entity);
                componentManager.RegisterAllComponents(entityHandle);
            }
        }

    }

}