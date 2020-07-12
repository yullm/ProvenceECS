using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{

    public class WorldRegistrationComplete : ProvenceEventArgs{
        public World world;
        public WorldRegistrationComplete(World world){
            this.world = world;
        }
    }
    
    [DontDisplayInEditor]
    public class World {
        
        public string id;
        public string worldName;
        
        public EntityManager entityManager;
        public ComponentManager componentManager;
        public EventManager<ProvenceEventArgs> eventManager;
        public SystemManager systemManager;
        [JsonIgnore]
        public SwitchManager switchManager;
        public GameObjectManager gameObjectManager;
    
        public World(){
            this.id = System.Guid.NewGuid().ToString();
            this.worldName = "New World";
            this.entityManager = new EntityManager(this);
            this.componentManager = new ComponentManager(this);
            this.eventManager = new EventManager<ProvenceEventArgs>(this);
            this.systemManager = new SystemManager(this);
            this.switchManager = new SwitchManager(this);
            this.gameObjectManager = new GameObjectManager(this);
        }

        public void Initialize(){
            gameObjectManager.Initialize();
            systemManager.Initialize();
            eventManager.Raise<WorldRegistrationComplete>(new WorldRegistrationComplete(this));
        }

        public EntityHandle CreateEntity(){
            return entityManager.CreateEntity();
        }

        public void RemoveEntity(EntityHandle entityHandle){
            gameObjectManager.RemoveGameObject(entityHandle);
            entityManager.RemoveEntity(entityHandle);
            componentManager.RemoveEntityEntries(entityHandle);
        }

        public EntityHandle LookUpEntity(Entity entity){
            return entityManager.LookUpEntity(entity);
        }

        public List<EntityHandle> LookUpAllEntities(){
            return entityManager.LookUpAllEntities();
        }

        public EntityHandle LookUpEntityByComponent<T>(T component) where T : ProvenceComponent{
            return componentManager.GetEntityByComponent<T>(component);
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent, new(){
            return componentManager.AddComponent<T>(entityHandle);
        }

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle, T component) where T : ProvenceComponent{
            return componentManager.AddComponent<T>(entityHandle,component);
        }

        public ComponentHandle<T> GetOrCreateComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent, new(){
            return componentManager.GetOrCreateComponent<T>(entityHandle);
        }

        public ComponentHandle<T> GetComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            return componentManager.GetComponent<T>(entityHandle);
        }

        public List<ComponentHandle<ProvenceComponent>> GetAllComponents(EntityHandle entityHandle){
            return componentManager.GetAllComponents(entityHandle);
        }

        public List<ComponentHandle<ProvenceComponent>> GetAllComponents(Entity entity){
            return componentManager.GetAllComponents(LookUpEntity(entity));
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            componentManager.RemoveComponent<T>(entityHandle);
        }

        public T AddSystem<T>() where T : ProvenceSystem, new(){
            return systemManager.AddSystem<T>();
        }

        public T GetSystem<T>() where T : ProvenceSystem{
            return systemManager.GetSystem<T>();
        }

        public T GetOrAddSystem<T>() where T : ProvenceSystem, new(){
            return systemManager.GetOrAddSystem<T>();
        }

        public void RemoveSystem<T>() where T : ProvenceSystem{
            systemManager.RemoveSystem<T>();
        }

        public GameObject AddGameObject(EntityHandle entityHandle){
            return gameObjectManager.AddGameObject(entityHandle);
        }

        public GameObject GetGameObject(EntityHandle entityHandle){
            return gameObjectManager.GetGameObject(entityHandle);
        }

        public void RemoveGameObject(EntityHandle entityHandle){
            gameObjectManager.RemoveGameObject(entityHandle);
        }

        public void Organize(){
            componentManager.Organize();
        }
    }

}