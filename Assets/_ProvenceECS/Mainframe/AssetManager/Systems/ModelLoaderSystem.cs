using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System.Threading.Tasks;

namespace ProvenceECS.Mainframe{

    public class ModelLoaderSystem : ProvenceSystem{
        
        protected Dictionary<Entity,ComponentHandle<Model>> modelCache;

        public ModelLoaderSystem(){
            this.modelCache = new Dictionary<Entity, ComponentHandle<Model>>();
        }

        public override void Initialize(WorldRegistrationComplete args){
            world.eventManager.AddListener<CacheIntegrityChange>(IntegrityCheck);
            world.eventManager.RemoveListener<ComponentAdded<Model>>(ModelComponentAdded);
            world.eventManager.AddListener<ComponentAdded<Model>>(ModelComponentAdded);
            //world.eventManager.AddListener<AllSystemsReadyEvent>(SetInitial);
            world.eventManager.Raise<SystemReadyEvent>(new SystemReadyEvent(this));
        }

        /* protected void SetInitial(AllSystemsReadyEvent args){
            if(!cacheIsSafe) GatherCache();
            foreach(ComponentHandle<Model> handle in modelCache.Values){
                LoadModel(handle.entity);
            }
        } */

        protected void ModelComponentAdded(ComponentAdded<Model> args){
            if(args.handle != null && args.handle.entity != null) LoadModel(args.handle.entity);          
        }

        protected void LoadModel(Entity entity){
            if(!cacheIsSafe) GatherCache();
            if(modelCache.ContainsKey(entity)){
                EntityHandle entityHandle = world.LookUpEntity(entity);
                GameObject entityObj = entityHandle.AddGameObject();
                entityObj.Clear();
                GameObject asset = ProvenceManager.AssetManager.LoadAsset<GameObject>(modelCache[entity].component.address);
                Vector3 offset = new Vector3();
                if(ProvenceManager.AssetManager.addressCache.ContainsKey(modelCache[entity].component.address))
                    offset = ProvenceManager.AssetManager.addressCache[modelCache[entity].component.address].offset;
                if(asset != null)
                    modelCache[entity].component.root = Object.Instantiate(asset,entityObj.transform.position + offset,entityObj.transform.rotation,entityObj.transform);
            }
        }

        public override void GatherCache(){
            modelCache = world.componentManager.GetAllComponentsAsDictionary<Model>();
            cacheIsSafe = true;
        }

        public override void IntegrityCheck(CacheIntegrityChange args){
            if(args.type == typeof(Model)) cacheIsSafe = false;
        }
    }

}