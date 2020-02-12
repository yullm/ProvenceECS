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
        public GameObject gameObject;

        public EntityHandle(World world){
            this.world = world;
            this.entity = null;
            this.gameObject = null;
        }

        public EntityHandle(Entity entity, World world, GameObject gameObject){
            this.world = world;
            this.entity = entity;
            this.gameObject = gameObject;
        }

        public ComponentHandle<T> AddComponent<T>(params object[] paramList) where T : Component{
            return world.AddComponent<T>(this,paramList);
        }

        public ComponentHandle<T> RegisterComponent<T>() where T : Component{
            return world.RegisterComponent<T>(this);
        }

        public ComponentHandle<T> GetComponent<T>() where T : Component{
            return world.GetComponentByEntity<T>(this);
        }

        public void RemoveComponent<T>() where T : Component{
            world.RemoveComponent<T>(this);
        }

        public void RemoveComponents<T>() where T : Component{
            world.RemoveComponents<T>(this);
        }

        public void RemoveComponent<T>(T component) where T : Component{
            world.RemoveComponent<T>(this, component);
        }

        public void Destroy(){
            world.RemoveEntity(this);
        }
    }
}