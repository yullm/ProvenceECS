using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldFixedUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public static WorldFixedUpdateEvent CreateInstance(World world, float time){
            WorldFixedUpdateEvent instance = ScriptableObject.CreateInstance<WorldFixedUpdateEvent>();
            instance.world = world;
            instance.time = time;
            return instance;
        }
    }    

}