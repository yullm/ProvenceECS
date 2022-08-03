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
        protected HashSet<Entity> entities;

        public EntityManager(World world){
            this.world = world;
            this.entities = new HashSet<Entity>();
        }

        public void Destroy(){
            entities.Clear();
        }

        public EntityHandle CreateEntity(string name = ""){
            Entity entity = new Entity();
            entities.Add(entity);
            EntityHandle entityHandle = new EntityHandle(entity, world);
            entityHandle.AddComponent<Name>(new Name(!name.Equals("") ? name : entity.ToString()));            
            return entityHandle;
        }

        public EntityHandle AddEntity(Entity entity, string name = ""){
            EntityHandle entityHandle = new EntityHandle(entity, world);
            if(!entities.Contains(entity)){
                entities.Add(entity);
                entityHandle.AddComponent<Name>(new Name(!name.Equals("") ? name : entity.ToString()));   
            }
            return entityHandle;
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