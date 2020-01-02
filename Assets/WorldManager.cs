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
            foreach(World world in startingWorlds) RegisterExistingWorld(world);
        }

        public void CreateNewWorld(string worldName){

            GameObject go = new GameObject("World: " + worldName);
            go.tag = "World";
            go.transform.parent = transform;
            World world = go.AddComponent<World>();
            world.worldName = worldName;
            world.manager = this;
            world.InitWorld();        
        }

        public void RegisterExistingWorld(World world){
            world.id = GetNewWorldID();
            world.manager = this;
            worlds[world.id] = world;
            world.InitWorld();
            world.systemManager.InitializeSystems();
            world.RegisterInitialEntities();
            world.eventManager.Raise<WorldRegistrationComplete>(new WorldRegistrationComplete(world));
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