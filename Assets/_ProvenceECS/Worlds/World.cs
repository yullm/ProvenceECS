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
            componentManager.Initialize();
            systemManager.Initialize();
            eventManager.Raise<WorldRegistrationComplete>(new WorldRegistrationComplete(this));
        }

        public EntityHandle CreateEntity(string name = ""){
            return entityManager.CreateEntity(name);
        }

        public void RemoveEntity(Entity entity){
            componentManager.RemoveEntityEntries(entity);
            gameObjectManager.RemoveGameObject(entity);
            entityManager.RemoveEntity(entity);
        }

        public EntityHandle LookUpEntity(Entity entity){
            return entityManager.LookUpEntity(entity);
        }

        public List<EntityHandle> LookUpAllEntities(){
            return entityManager.LookUpAllEntities();
        }

        public EntityHandle DuplicateEntity(Entity entity){
            EntityHandle duplicateHandle = CreateEntity();
            gameObjectManager.DuplicateGameObject(entity, duplicateHandle.entity);
            componentManager.CopyComponentsToOtherEntity(entity, duplicateHandle.entity);
            return duplicateHandle;
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity) where T : ProvenceComponent, new(){
            return componentManager.AddComponent<T>(entity);
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity, T component) where T : ProvenceComponent{
            return componentManager.AddComponent<T>(entity,component);
        }

        public ComponentHandle<T> GetOrCreateComponent<T>(Entity entity) where T : ProvenceComponent, new(){
            return componentManager.GetOrCreateComponent<T>(entity);
        }

        public ComponentHandle<T> GetComponent<T>(Entity entity) where T : ProvenceComponent{
            return componentManager.GetComponent<T>(entity);
        }

        public HashSet<ComponentHandle<ProvenceComponent>> GetAllComponents(Entity entity){
            return componentManager.GetAllComponents(entity);
        }

        public void RemoveComponent<T>(Entity entity) where T : ProvenceComponent{
            componentManager.RemoveComponent<T>(entity);
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

        public GameObject AddGameObject(Entity entity){
            return gameObjectManager.AddGameObject(entity);
        }

        public GameObject GetGameObject(Entity entity){
            return gameObjectManager.GetGameObject(entity);
        }

        public GameObject SetGameObject(Entity entity, GameObject gameObject){
            return gameObjectManager.SetGameObject(entity,gameObject);
        }

        public void RemoveGameObject(Entity entity){
            gameObjectManager.RemoveGameObject(entity);
        }

        public void Organize(){
            componentManager.Organize();
        }
    }

}