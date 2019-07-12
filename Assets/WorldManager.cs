using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public World[] worlds = new World[0];
    public World activeWorld;

    public void CreateNewWorld(string worldName){

        GameObject go = new GameObject("World: " + worldName);
        go.tag = "World";
        go.transform.parent = transform;
        World world = go.AddComponent<World>();
        world.id = GetNewWorldID();
        world.worldName = worldName;
        world.manager = this;
        worlds[world.id] = world;

        EntityManager em = world.entityManager = go.AddComponent<EntityManager>();
        em.world = world;
        ComponentManager cm = world.ComponentManager = go.AddComponent<ComponentManager>();
        cm.world = world;
        
    }

    public int GetNewWorldID(){
        for(int i = 0; i < worlds.Length; i++){
            if(worlds[i] == null) return i;
        }
        print("No room");
        World[] newWorlds = new World[worlds.Length + 1];
        for(int i = 0; i < worlds.Length; i++){
            newWorlds[i] = worlds[i];
        }
        worlds = newWorlds;
        return worlds.Length - 1;
    }
}
