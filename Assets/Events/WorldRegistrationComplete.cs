using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    
    public class WorldRegistrationComplete : ProvenceEventArgs{
        public World world;
        public static WorldRegistrationComplete CreateInstance(World world){
            WorldRegistrationComplete instance = ScriptableObject.CreateInstance<WorldRegistrationComplete>();
            instance.world = world;
            return instance;
        }
    }

}