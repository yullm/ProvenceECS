using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class EntityManager : MonoBehaviour
    {   

        public World world;
        public Dictionary<Entity,GameObject> entities = new Dictionary<Entity, GameObject>();
        public List<Entity> availableEntities = new List<Entity>();

        public EntityHandle CreateEntity(){
            Entity entity;
            EntityHandle entityHandle = new EntityHandle(world);
            if(availableEntities.Count > 0 && availableEntities[0] != null){
                entity = availableEntities[0];
                try{
                    entities[entity].SetActive(true);
                    availableEntities.Remove(entity);
                    entityHandle.entity = entity;
                    entityHandle.gameObject = entities[entity];
                }catch(KeyNotFoundException e){
                    Debug.Log("Missing keys caught and solved, " + e);
                    foreach(KeyValuePair<Entity,GameObject> kvp in entities){
                        if(kvp.Key == entity) {
                            availableEntities.Remove(entity);
                            kvp.Value.SetActive(true);
                            break;
                        }
                    }
                }  
            }else{
                GameObject obj = new GameObject("Entity:" + entities.Count);
                entity = obj.AddComponent<Entity>();
                obj.tag = "Entity";
                obj.transform.parent = this.gameObject.transform;
                entity.id = entities.Count;
                entities.Add(entity,obj);
                entityHandle.entity = entity;
                entityHandle.gameObject = obj;
                entityHandle.RegisterComponent<Transform>();
            }
            return entityHandle;
        }

        public EntityHandle RegisterEntity(Entity entity){
            EntityHandle entityHandle = new EntityHandle(world);
            entity.id = entities.Count;
            entity.gameObject.tag = "Entity";
            entities.Add(entity, entity.gameObject);
            entityHandle.entity = entity;
            entityHandle.gameObject = entity.gameObject;
            return entityHandle;
        }

        public void RemoveEntity(EntityHandle entityHandle){
            entityHandle.gameObject.SetActive(false);
            if(!availableEntities.Contains(entityHandle.entity)) availableEntities.Add(entityHandle.entity);
        }

        public void RemoveEntity(int id){
            foreach(KeyValuePair<Entity,GameObject> kvp in entities){
                if(kvp.Key.id == id) {
                    RemoveEntity(LookUpEntity(kvp.Key));
                    return;
                }
            }
        }

        public EntityHandle LookUpEntity(Entity entity){
            return new EntityHandle(entity, world, entities[entity]);
        }

        public List<EntityHandle> LookUpAllEntities(){
            List<EntityHandle> handles = new List<EntityHandle>();
            foreach(Entity entity in entities.Keys) handles.Add(new EntityHandle(entity,world,entities[entity]));
            return handles;
        }

        public void ClearEntities(){
            foreach(KeyValuePair<Entity,GameObject> kvp in entities){
                world.RemoveEntity(world.LookUpEntity(kvp.Key));
            }
            availableEntities.Clear();
            entities.Clear();       
        }
    }
    
}