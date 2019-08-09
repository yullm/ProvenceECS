using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldManager : MonoBehaviour
    {
        public World[] worlds = new World[0];
        public World activeWorld;

        public void Start(){
            World[] startingWorlds = GetComponentsInChildren<World>();
            foreach(World world in startingWorlds){
                RegisterExistingWorld(world);
                world.RegisterInitialEntities();
            }
        }

        public void CreateNewWorld(string worldName){

            GameObject go = new GameObject("World: " + worldName);
            go.tag = "World";
            go.transform.parent = transform;
            World world = go.AddComponent<World>();
            world.id = GetNewWorldID();
            world.worldName = worldName;
            world.manager = this;
            worlds[world.id] = world;

            /* not necessary???
                EntityManager em = world.entityManager = go.AddComponent<EntityManager>();
                em.world = world;
                ComponentManager cm = world.componentManager = go.AddComponent<ComponentManager>();
                cm.world = world;
            */
        }

        public void RegisterExistingWorld(World world){
            world.id = GetNewWorldID();
            world.manager = this;
            worlds[world.id] = world;
            if(world.entityManager == null) world.entityManager = world.GetComponent<EntityManager>();
            if(world.componentManager == null) world.componentManager = world.GetComponent<ComponentManager>();
            if(world.eventManager == null) world.eventManager = world.GetComponent<EventManager>();
            if(world.systemManager == null) world.systemManager = world.GetComponent<SystemManager>();
            world.entityManager.world = world;
            world.componentManager.world = world;
            world.eventManager.world = world;
            world.systemManager.world = world;
            world.systemManager.BroadcastWorld();
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
}