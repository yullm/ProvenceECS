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
    
        public World(){
            this.id = System.Guid.NewGuid().ToString();
            this.worldName = "New World";
            this.entityManager = new EntityManager(this);
            this.componentManager = new ComponentManager(this);
            this.eventManager = new EventManager<ProvenceEventArgs>(this);
            this.systemManager = new SystemManager(this);
        }

        public void Initialize(){          
            componentManager.Initialize();
            systemManager.Initialize();
            eventManager.Raise<WorldRegistrationComplete>(new WorldRegistrationComplete(this));
        }

        public void Destroy(){
            systemManager.Destroy();
            componentManager.Destroy();
            entityManager.Destroy();
            eventManager.ClearListeners();
        }

        public EntityHandle CreateEntity(string name = ""){
            return entityManager.CreateEntity(name);
        }

        public EntityHandle AddEntity(Entity entity, string name = ""){
            return entityManager.AddEntity(entity,name);
        }

        public void RemoveEntity(Entity entity){
            componentManager.RemoveEntityEntries(entity);
            entityManager.RemoveEntity(entity);
        }

        public EntityHandle LookUpEntity(Entity entity){
            return entityManager.LookUpEntity(entity);
        }

        public List<EntityHandle> LookUpAllEntities(){
            return entityManager.LookUpAllEntities();
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity) where T : ProvenceComponent, new(){
            return componentManager.AddComponent<T>(entity);
        }

        public ComponentHandle<T> AddComponent<T>(Entity entity, T component) where T : ProvenceComponent{
            return componentManager.AddComponent<T>(entity,component);
        }

        public void AddComponentSet(Entity entity, HashSet<ProvenceComponent> set){
            componentManager.AddComponentSet(entity, set);
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

        public void RemoveComponent<T>(Entity entity, T Component) where T : ProvenceComponent{
            componentManager.RemoveComponent<T>(entity);
        }

        public T AddSystem<T>() where T : ProvenceSystem, new(){
            return systemManager.AddSystem<T>();
        }

        public void AddSystem<T>(T system, bool overwrite = true) where T : ProvenceSystem{
            systemManager.AddSystem(system, overwrite);
        }

        public T GetSystem<T>() where T : ProvenceSystem{
            return systemManager.GetSystem<T>();
        }

        public T GetOrAddSystem<T>() where T : ProvenceSystem, new(){
            return systemManager.GetOrAddSystem<T>();
        }

        public void RemoveSystem<T>(SystemDestructionType destructionType = SystemDestructionType.SYSTEM_REMOVAL) where T : ProvenceSystem{
            systemManager.RemoveSystem<T>(destructionType);
        }

        public void RemoveSystem<T>(T system, SystemDestructionType destructionType = SystemDestructionType.SYSTEM_REMOVAL) where T : ProvenceSystem{
            systemManager.RemoveSystem<T>(destructionType);
        }

        public void TransferSystem<T>(World toWorld) where T : ProvenceSystem, new(){
            T system = GetSystem<T>();
            if(system == null) return;
            RemoveSystem<T>(SystemDestructionType.TRANSFER);
            toWorld.AddSystem(system);
        }

        public void TransferSystem<T>(World toWorld, T system) where T : ProvenceSystem{
            RemoveSystem(system,SystemDestructionType.TRANSFER);
            toWorld.AddSystem(system);
        }

        public void AddEntityDictionary(Dictionary<Entity,HashSet<ProvenceComponent>> dict){
            foreach(Entity entity in dict.Keys){
                entityManager.AddEntity(entity);
            }
            foreach(KeyValuePair<Entity,HashSet<ProvenceComponent>> kvp in dict){
                componentManager.AddComponentSet(kvp.Key,kvp.Value);
            }
        }

    }

}