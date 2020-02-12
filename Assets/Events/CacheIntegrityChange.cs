using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class CacheIntegrityChange : ProvenceEventArgs{
        public World world;
        public System.Type type;
        public static CacheIntegrityChange CreateInstance(World world, System.Type type){
            CacheIntegrityChange instance = ScriptableObject.CreateInstance<CacheIntegrityChange>();
            instance.world = world;
            instance.type = type;
            return instance;
        }
    }

}