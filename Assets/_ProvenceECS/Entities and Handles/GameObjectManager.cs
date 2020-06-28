using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{

    public class GameObjectManager {

        public World world;
        protected Dictionary<Entity,GameObject> gameObjectDictionary;
        
        public GameObjectManager(World world){
            this.world = world;
            this.gameObjectDictionary = new Dictionary<Entity, GameObject>();
        }

        public void Initialize(){
            foreach(GameObject gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>()){
                if(world.LookUpEntity((Entity)gameObject.name) != null)
                    gameObjectDictionary[(Entity)gameObject.name] = gameObject;
            }
        }

        public GameObject this[EntityHandle entityHandle]{
            get{
                if(gameObjectDictionary.ContainsKey(entityHandle.entity)) return gameObjectDictionary[entityHandle.entity];
                return null;
            }
        }

        public GameObject GetGameObject(EntityHandle entityHandle){
            if(gameObjectDictionary.ContainsKey(entityHandle.entity)){
                if(gameObjectDictionary[entityHandle.entity] == null){
                    gameObjectDictionary.Remove(entityHandle.entity);
                    return null;
                }
                return gameObjectDictionary[entityHandle.entity];
            }        
            return null;
        }

        public GameObject AddGameObject(EntityHandle entityHandle){
            if(gameObjectDictionary.ContainsKey(entityHandle.entity) && gameObjectDictionary[entityHandle.entity] != null)
                return gameObjectDictionary[entityHandle.entity];     
            
            GameObject go = new GameObject(entityHandle.entity.ToString());
            gameObjectDictionary[entityHandle.entity] = go;
            world.eventManager.Raise<CacheIntegrityChange>(new CacheIntegrityChange(typeof(GameObject)));
            return go;
        }

        public void RemoveGameObject(EntityHandle entityHandle){
            if(gameObjectDictionary.ContainsKey(entityHandle.entity)){
                UnityEngine.Object.DestroyImmediate(gameObjectDictionary[entityHandle.entity]);
                world.eventManager.Raise<CacheIntegrityChange>(new CacheIntegrityChange(typeof(GameObject)));                
            }
        }

        public void Clear(){
            foreach(GameObject gameObject in gameObjectDictionary.Values){
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
        
    }

}