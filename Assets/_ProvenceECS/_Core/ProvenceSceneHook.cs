using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProvenceECS{
    
    //loaded worlds will have their own hook if it is a scene which the update events get sent from - maybe not the best

    public class ProvenceSceneHook : MonoBehaviour{
        public string id = "";
        protected World world;

        void Start(){
            if(!ProvenceManager.Instance.worlds.ContainsKey(id))
                ProvenceManager.Instance.AddWorld(id);
            world = ProvenceManager.Instance.worlds[id];
        }

        void Update(){
            if(world != null) world.eventManager.Raise(new WorldUpdateEvent(world, Time.deltaTime));
        }

        void LateUpate(){
            if(world != null) world.eventManager.Raise(new WorldLateUpdateEvent(world, Time.deltaTime));
        }

        void FixedUpdate(){
            if(world != null) world.eventManager.Raise(new WorldFixedUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnGUI(){
            if(world != null) world.eventManager.Raise(new WorldGUIUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnDrawGizmos(){
            if(world != null) world.eventManager.Raise(new WorldDrawGizmosUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnDestroy(){
            if(world != null) world.eventManager.Raise(new WorldSafetyDestroy(world, Time.fixedDeltaTime));
        }

        void OnApplicationQuit(){
            if(world != null) world.eventManager.Raise(new WorldSafetyDestroy(world, Time.fixedDeltaTime));
        }

    }
    
}