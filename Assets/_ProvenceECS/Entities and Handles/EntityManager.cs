using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{
    [Serializable]
    public class EntityManager{   
        public World world;
        [JsonProperty]
        protected List<Entity> entities;

        public EntityManager(World world){
            this.world = world;
            this.entities = new List<Entity>();
        }

        public EntityHandle CreateEntity(){
            Entity entity;
            EntityHandle entityHandle = new EntityHandle(world);
            entity = new Entity();
            entities.Add(entity);
            entityHandle.entity = entity;
            entityHandle.AddComponent<Name>(new Name(entity.ToString()));            
            return entityHandle;
        }

        public void RemoveEntity(EntityHandle entityHandle){
            RemoveEntity(entityHandle.entity);
        }

        public void RemoveEntity(Entity entity){
            if(!entities.Contains(entity)) return;
            entities.Remove(entity);
        }

        public EntityHandle LookUpEntity(Entity entity){
            if(!entities.Contains(entity)) return null;
            return new EntityHandle(entity, world);
        }

        public List<EntityHandle> LookUpAllEntities(){
            List<EntityHandle> handles = new List<EntityHandle>();
            foreach(Entity entity in entities) handles.Add(LookUpEntity(entity));
            return handles;
        }

        public void ClearEntities(){
            entities.Clear();
        }
    }
    
}