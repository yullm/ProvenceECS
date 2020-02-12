using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public static WorldUpdateEvent CreateInstance(World world, float time){
            WorldUpdateEvent instance = ScriptableObject.CreateInstance<WorldUpdateEvent>();
            instance.world = world;
            instance.time = time;
            return instance;
        }
    }

}