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

        public Entity CreateEntity(){
            return entityManager.CreateEntity();
        }

        public void RemoveEntity(Entity entity){
            entityManager.RemoveEntity(entity);
            componentManager.RemoveEntityEntries(entity);
        }

        public GameObject LookUpEntity(Entity entity){
            return entityManager.LookUpEntity(entity);
        }

        public T AddComponent<T>(Entity entity) where T : ProvenceComponent{
            return componentManager.AddComponent<T>(entity);
        }

        public void RemoveComponent<T>(Entity entity) where T : ProvenceComponent{
            entityManager.RemoveEntity(entity);
        }

    }

}