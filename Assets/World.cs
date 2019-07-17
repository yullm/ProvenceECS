using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class World : MonoBehaviour
    {
        public int id;
        public string worldName;
        public WorldManager manager;
        public EntityManager entityManager;
        public ComponentManager componentManager;

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

        public ComponentHandle<T> AddComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            return componentManager.AddComponent<T>(entityHandle);
        }

        public void RemoveComponent<T>(EntityHandle entityHandle) where T : ProvenceComponent{
            componentManager.RemoveComponent<T>(entityHandle);
        }

    }

}