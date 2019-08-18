using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldUpdateEvent : ProvenceEventArgs{
        public World world;
    }

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

        void Update(){
            if(eventManager) eventManager.Raise<WorldUpdateEvent>(new WorldUpdateEvent(){world = this});
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

        public void RegisterInitialEntities(){
            Entity[] initialEntities = GetComponentsInChildren<Entity>();
            foreach(Entity entity in initialEntities){
                EntityHandle entityHandle = entityManager.RegisterEntity(entity);
                componentManager.RegisterAllComponents(entityHandle);
            }
        }

    }

}