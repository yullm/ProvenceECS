using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    [ExecuteInEditMode]
    [System.Serializable]
    public class Entity : MonoBehaviour{
        public int id;
    }

    public class EntityHandle{
        public Entity entity;
        public World world;
        public EntityManager manager;
        public GameObject gameObject;

        public ComponentHandle<T> AddComponent<T>(params object[] paramList) where T : Component{
            return world.AddComponent<T>(this,paramList);
        }

        public ComponentHandle<T> RegisterComponent<T>() where T : Component{
            return world.RegisterComponent<T>(this);
        }

        public void RemoveComponent<T>() where T : Component{
            world.RemoveComponent<T>(this);
        }

        public void Destroy(){
            world.RemoveEntity(this);
        }
    }
}