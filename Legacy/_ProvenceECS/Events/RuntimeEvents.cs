using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class WorldUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public WorldUpdateEvent(World world, float time){
            this.world = world;
            this.time = time;
        }
    }

    public class WorldLateUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public WorldLateUpdateEvent(World world, float time){
            this.world = world;
            this.time = time;
        }
    }

    public class WorldFixedUpdateEvent : ProvenceEventArgs{
        public World world;
        public float time;
        public WorldFixedUpdateEvent(World world, float time){
            this.world = world;
            this.time = time;
        }
    }   

    public class WorldGUIUpdateEvent : ProvenceEventArgs{

        public World world;
        public float time;
        public WorldGUIUpdateEvent(World world, float time){
            this.world = world;
            this.time = time;
        }
    
    }

    public class WorldDrawGizmosUpdateEvent : ProvenceEventArgs{

        public World world;
        public float time;
        public WorldDrawGizmosUpdateEvent(World world, float time){
            this.world = world;
            this.time = time;
        }
    
    }

    public class EditorPersistanceUpdateEvent : ProvenceEventArgs{
        public World world;
        public EditorPersistanceUpdateEvent(World world){
            this.world = world;
        }
    }

    public class WorldSafetyDestroy : ProvenceEventArgs{
        public World world;
        public float time;
        public WorldSafetyDestroy(World world, float time){
            this.world = world;
            this.time = time;
        }
    }

    public class EditorSelectEntities : ProvenceEventArgs{

        public World world;
        public HashSet<Entity> entities;

        public EditorSelectEntities(){
            this.world = null;
            this.entities = new HashSet<Entity>();
        }

        public EditorSelectEntities(World world, HashSet<Entity> entities){
            this.world = world;
            this.entities = entities;
        }
        
    }

}