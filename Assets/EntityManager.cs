using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityDictionary : SerializableDicitonary<Entity, GameObject>{};
public class EntityManager : MonoBehaviour
{   

    public int worldId;
    public EntityDictionary entities = new EntityDictionary();
    //Pooling old Entities
    public List<Entity> availableEntities = new List<Entity>();

    public Entity CreateEntity(){
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
        return entity;
    }

    public void RemoveEntity(Entity entity){
        entities[entity].SetActive(false);
        if(!availableEntities.Contains(entity)) availableEntities.Add(entity);
        //availableEntities.Sort();
    }

    public void RemoveEntity(int id){
        foreach(KeyValuePair<Entity,GameObject> kvp in entities){
            if(kvp.Key == id) {
                RemoveEntity(kvp.Key);
                return;
            }
        }
    }

    public void ClearEntities(){
        foreach(KeyValuePair<Entity,GameObject> kvp in entities){
            DestroyImmediate(kvp.Value);
        }
        availableEntities.Clear();
        entities.Clear();       
    }
}
