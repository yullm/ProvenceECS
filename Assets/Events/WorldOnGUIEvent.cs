using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldOnGUIEvent : ProvenceEventArgs{

        public World world;
        public float time;
        public static WorldOnGUIEvent CreateInstance(World world, float time){
            WorldOnGUIEvent instance = ScriptableObject.CreateInstance<WorldOnGUIEvent>();
            instance.world = world;
            instance.time = time;
            return instance;
        }
    
    }
}