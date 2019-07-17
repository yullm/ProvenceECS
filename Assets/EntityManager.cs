using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class EntityDictionary : SerializableDictionary<Entity, GameObject>{};

    public class EntityManager : MonoBehaviour
    {   

        public World world;
        public EntityDictionary entities = new EntityDictionary();
        public List<Entity> availableEntities = new List<Entity>();

        public EntityHandle CreateEntity(){
            Entity entity;
            if(availableEntities.Count > 0){
                entity = availableEntities[0];
                try{
                    entities[entity].SetActive(true);
                    availableEntities.Remove(entity);
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
                obj.tag = "Entity";
                obj.transform.parent = this.gameObject.transform;
                //REGISTER TRANSFORM WHEN COMPONENT MANAGER IS CREATED
                entity =  entities.Count;
                entities.Add(entity,obj);
            }
            return new EntityHandle(){entity = entity, world = world, manager = this, gameObject = entities[entity]};
        }

        public void RemoveEntity(EntityHandle entityHandle){
            entityHandle.gameObject.SetActive(false);
            if(!availableEntities.Contains(entityHandle.entity)) availableEntities.Add(entityHandle.entity);
        }

        public void RemoveEntity(int id){
            foreach(KeyValuePair<Entity,GameObject> kvp in entities){
                if(kvp.Key == id) {
                    RemoveEntity(LookUpEntity(kvp.Key));
                    return;
                }
            }
        }

        public EntityHandle LookUpEntity(Entity entity){
            return new EntityHandle(){entity = entity, world = world, manager = this, gameObject = entities[entity]};
        }

        public void ClearEntities(){
            foreach(KeyValuePair<Entity,GameObject> kvp in entities){
                DestroyImmediate(kvp.Value);
            }
            availableEntities.Clear();
            entities.Clear();       
        }
    }
    
}