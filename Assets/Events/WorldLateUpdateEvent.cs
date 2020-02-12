using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    
    public class WorldLateUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public static WorldLateUpdateEvent CreateInstance(World world, float time){
            WorldLateUpdateEvent instance = ScriptableObject.CreateInstance<WorldLateUpdateEvent>();
            instance.world = world;
            instance.time = time;
            return instance;
        }
    }

}