using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{

    public class UnityGameObject : ProvenceComponent{
        public GameObject gameObject;
        public Vector3 position;
        public Quaternion rotation;

        public UnityGameObject(){
            this.gameObject = null;
            this.position = new Vector3();
            this.rotation = new Quaternion();
        }

        public UnityGameObject(GameObject gameObject) : this(){
            this.gameObject = gameObject;
        }

        public override ProvenceComponent Clone(){
            UnityGameObject comp = (UnityGameObject) base.Clone();
            comp.gameObject = null;
            return comp;            
        }
    }

    public class UnityGameObjectSystem : ProvenceSystem{

        protected ComponentCache<UnityGameObject> objectCache;

        public UnityGameObjectSystem(){
            objectCache = new ComponentCache<UnityGameObject>();
        }

        public override void Destroy(SystemDestructionType destructionType){
            DestroyAllObjects();
            DeregisterEventListeners();
        }

        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<WakeSystemEvent>(LoadGameObjects,-2);
            world.eventManager.AddListener<WorldUpdateEvent>(Tick);
            world.eventManager.AddListener<ComponentAdded<UnityGameObject>>(ComponentAdded);
            world.eventManager.AddListener<ComponentRemoved<UnityGameObject>>(ComponentRemoved);
            objectCache.StandardRegistration(world);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<WakeSystemEvent>(LoadGameObjects);
            world.eventManager.RemoveListener<WorldUpdateEvent>(Tick);
            world.eventManager.RemoveListener<ComponentAdded<UnityGameObject>>(ComponentAdded);
            world.eventManager.RemoveListener<ComponentRemoved<UnityGameObject>>(ComponentRemoved);
            objectCache.StandardDeregistration(world);
        }

        protected void LoadGameObjects(WakeSystemEvent args){
            foreach(GameObject gameObject in UnityEngine.Object.FindObjectsOfType<GameObject>()){
                EntityHandle entityHandle = world.LookUpEntity((Entity)gameObject.name);
                if(entityHandle != null){
                    ComponentHandle<UnityGameObject> objectHandle = entityHandle.GetOrCreateComponent<UnityGameObject>();
                    objectHandle.component.gameObject = gameObject;
                }
            }
        }

        protected void ComponentAdded(ComponentAdded<UnityGameObject> args){
            /* GameObject gameObject = args.handle.component.gameObject;
            if(gameObject != null)
                gameObject.name = args.handle.entity.ToString();
            else
                gameObject = GameObject.Find(args.handle.entity.ToString());
            if(gameObject != null){        
                args.handle.component.gameObject = gameObject;
            }else{                
                gameObject = new GameObject(args.handle.entity.ToString());
                args.handle.component.gameObject = gameObject;
            }            
            gameObject.transform.position = args.handle.component.position;
            gameObject.transform.rotation = args.handle.component.rotation; */
            GameObject gameObject = args.handle.component.gameObject;
            if(gameObject != null)
                gameObject.name = args.handle.entity.ToString();
            else
                gameObject = GameObject.Find(args.handle.entity.ToString());
            if(gameObject != null){        
                args.handle.component.gameObject = gameObject;
                args.handle.component.position = gameObject.transform.position;
                args.handle.component.rotation = gameObject.transform.rotation;
            }else{                
                gameObject = new GameObject(args.handle.entity.ToString());
                args.handle.component.gameObject = gameObject;
                gameObject.transform.position = args.handle.component.position;
                gameObject.transform.rotation = args.handle.component.rotation;
            }
        }

        protected void ComponentRemoved(ComponentRemoved<UnityGameObject> args){
            if(args.handle.component.gameObject != null)
                UnityEngine.Object.DestroyImmediate(args.handle.component.gameObject);
        }

        protected void Tick(WorldUpdateEvent args){
            foreach(ComponentHandle<UnityGameObject> objectHandle in objectCache.Values.ToSet()){
                if(objectHandle.component.gameObject != null){
                    objectHandle.component.position = objectHandle.component.gameObject.transform.position;
                    objectHandle.component.rotation = objectHandle.component.gameObject.transform.rotation;
                }else{
                    GameObject gameObject = new GameObject(objectHandle.entity.ToString());
                    objectHandle.component.gameObject = gameObject;
                    gameObject.transform.position = objectHandle.component.position;
                    gameObject.transform.rotation = objectHandle.component.rotation;
                }
            }
        }

        protected void DestroyAllObjects(){
            foreach(ComponentHandle<UnityGameObject> objectHandle in objectCache.Values.ToSet()){
                UnityEngine.Object.DestroyImmediate(objectHandle.component.gameObject);
            }
        }

        

    }

}