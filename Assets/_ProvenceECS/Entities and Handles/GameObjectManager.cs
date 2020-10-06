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

        public GameObject this[Entity entity]{
            get{
                if(gameObjectDictionary.ContainsKey(entity)) return gameObjectDictionary[entity];
                return null;
            }
        }

        public GameObject GetGameObject(Entity entity){
            if(entity == null) return null;
            if(gameObjectDictionary.ContainsKey(entity)){
                if(gameObjectDictionary[entity] == null){
                    gameObjectDictionary.Remove(entity);
                    return null;
                }
                return gameObjectDictionary[entity];
            }        
            return null;
        }

        public GameObject AddGameObject(Entity entity){
            if(gameObjectDictionary.ContainsKey(entity) && gameObjectDictionary[entity] != null)
                return gameObjectDictionary[entity];     
            
            GameObject go = new GameObject(entity.ToString());
            gameObjectDictionary[entity] = go;
            world.eventManager.Raise<CacheIntegrityChange<GameObject>>(new CacheIntegrityChange<GameObject>(world));
            return go;
        }

        public GameObject SetGameObject(Entity entity, GameObject gameObject){
            if(gameObjectDictionary.ContainsKey(entity))
                UnityEngine.Object.DestroyImmediate(gameObjectDictionary[entity]);
            gameObject.name = entity.ToString();
            gameObjectDictionary[entity] = gameObject;
            world.eventManager.Raise<CacheIntegrityChange<GameObject>>(new CacheIntegrityChange<GameObject>(world));
            return gameObject;
        }

        public void RemoveGameObject(Entity entity){
            if(gameObjectDictionary.ContainsKey(entity)){
                UnityEngine.Object.DestroyImmediate(gameObjectDictionary[entity]);
                world.eventManager.Raise<CacheIntegrityChange<GameObject>>(new CacheIntegrityChange<GameObject>(world));                
            }
        }
        
        public GameObject DuplicateGameObject(Entity original, Entity copy){
            GameObject originalObject = GetGameObject(original);
            if(originalObject == null) return null;
            GameObject copyObject = AddGameObject(copy);
            copyObject.transform.position = originalObject.transform.position;
            copyObject.transform.rotation = originalObject.transform.rotation;
            copyObject.transform.localScale = originalObject.transform.localScale;
            return copyObject;
        }

        public void Clear(){
            foreach(GameObject gameObject in gameObjectDictionary.Values){
                UnityEngine.Object.DestroyImmediate(gameObject);
            }
        }
        
    }

}