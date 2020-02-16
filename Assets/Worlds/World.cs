using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [RequireComponent(typeof(EntityManager),typeof(ComponentManager),typeof(EventManager))]
    [RequireComponent(typeof(SystemManager),typeof(SwitchManager))]
    public class World : MonoBehaviour{
        public enum GameState {MENU, GAMEPLAY};
        public GameState gameState = GameState.GAMEPLAY;
        public int id;
        public string worldName;
        public WorldManager manager;
        public EntityManager entityManager;
        public ComponentManager componentManager;
        public EventManager eventManager;
        public SystemManager systemManager;
        public SwitchManager switchManager;
    
        public void InitWorld(){
            if(entityManager == null) entityManager = GetComponent<EntityManager>();
            if(componentManager == null) componentManager = GetComponent<ComponentManager>();
            if(eventManager == null) eventManager = GetComponent<EventManager>();
            if(systemManager == null) systemManager = GetComponent<SystemManager>();
            if(switchManager == null) switchManager = GetComponent<SwitchManager>();
            if(entityManager.world == null) entityManager.world = this;
            if(componentManager.world == null) componentManager.world = this;
            if(eventManager.world == null) eventManager.world = this;
            if(systemManager.world == null) systemManager.world = this;
            if(switchManager.world == null) switchManager.world = this;
        }

        void Update(){
            if(eventManager) eventManager.Raise<WorldUpdateEvent>(WorldUpdateEvent.CreateInstance(this, Time.time));
        }
        void LateUpdate(){
            if(eventManager) eventManager.Raise<WorldLateUpdateEvent>(WorldLateUpdateEvent.CreateInstance(this, Time.fixedTime));
        }

        void FixedUpdate(){
            if(eventManager) eventManager.Raise<WorldFixedUpdateEvent>(WorldFixedUpdateEvent.CreateInstance(this, Time.fixedTime));
        }

        void OnGUI(){
            if(eventManager) eventManager.Raise<WorldOnGUIEvent>(WorldOnGUIEvent.CreateInstance(this, Time.time));
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

        public List<EntityHandle> LookUpAllEntities(){
            return entityManager.LookUpAllEntities();
        }

        public EntityHandle LookUpEntityByComponent<T>(T component) where T : Component{
            return componentManager.GetEntityByComponent<T>(component);
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, params object[] paramList) where T : Component{
            return componentManager.AddComponent<T>(entityHandle,paramList);
        }

        public ComponentHandle<T> RegisterComponent<T>(EntityHandle entityHandle) where T : Component{
            return componentManager.RegisterComponent<T>(entityHandle);
        }

        public ComponentHandle<T> GetComponentByEntity<T>(EntityHandle entityHandle) where T : Component{
            return componentManager.GetComponentByEntity<T>(entityHandle);
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : Component{
            componentManager.RemoveComponent<T>(entityHandle);
        }

        public void RemoveComponent<T>(EntityHandle entityHandle, T component) where T : Component{
            componentManager.RemoveComponent<T>(entityHandle, component);
        }

        public void RemoveComponents<T>(EntityHandle entityHandle) where T : Component{
            componentManager.RemoveComponents<T>(entityHandle);
        }

        public void AddSystem<T>() where T : ProvenceSystem{
            systemManager.AddSystem<T>();
        }

        public ProvenceSystem GetSystem<T>() where T : ProvenceSystem{
            return systemManager.GetSystem<T>();
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