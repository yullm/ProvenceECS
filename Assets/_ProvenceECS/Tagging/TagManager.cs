using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ProvenceECS{
    public class TagManager{

        public World world;

        [JsonProperty] protected Dictionary<string,HashSet<Entity>> tagDictionary; 

        public TagManager(World world){
            this.world = world;
            this.tagDictionary = new Dictionary<string, HashSet<Entity>>();
        }

        public void AddTagToEntity(EntityHandle entityHandle, string tag){
            if(!tagDictionary.ContainsKey(tag)) tagDictionary[tag] = new HashSet<Entity>();
            tagDictionary[tag].Add(entityHandle.entity);
        }

        public void RemoveTagFromEntity(EntityHandle entityHandle, string tag){
            if(tagDictionary.ContainsKey(tag)) tagDictionary[tag].Remove(entityHandle.entity);
        }

        public List<EntityHandle> GetAllTaggedEntities(string tag){
            List<EntityHandle> handleList = new List<EntityHandle>();
            if(tagDictionary.ContainsKey(tag) && tagDictionary[tag] != null){
                foreach(Entity entity in tagDictionary[tag]){
                    EntityHandle handle = world.LookUpEntity(entity);
                    if(handle != null) handleList.Add(handle);
                }
            }
            return handleList;
        }
        
    }
}