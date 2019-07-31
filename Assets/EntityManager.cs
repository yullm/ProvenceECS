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
            EntityHandle entityHandle = new EntityHandle{world = world, manager = this};
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
                GameObject obj = new GameObject("Entity:" + (entities.Count > 0 ? entities.keys[entities.keys.Count - 1].id + 1 : 0));
                entity = obj.AddComponent<Entity>();
                obj.tag = "Entity";
                obj.transform.parent = this.gameObject.transform;
                entity.id = entities.Count > 0 ? entities.keys[entities.keys.Count - 1].id + 1 : 0;
                entities.Add(entity,obj);
                entityHandle.entity = entity;
                entityHandle.gameObject = obj;
                entityHandle.RegisterComponent<Transform>();
                
                entityHandle.AddComponent<TestComponentA>();
            }
            print(entities.Count);
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

        public void RemoveEntityPermanently(Entity entity){
            entities.Remove(entity);
        }

        public EntityHandle LookUpEntity(Entity entity){
            return new EntityHandle(){entity = entity, world = world, manager = this, gameObject = entities[entity]};
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