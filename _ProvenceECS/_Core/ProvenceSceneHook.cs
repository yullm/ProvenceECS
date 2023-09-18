using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProvenceECS{

    public class ProvenceSceneHook : MonoBehaviour{
        public string id = "";
        protected World world;

        void Start(){
            if(!ProvenceManager.Instance.worlds.ContainsKey(id))
                ProvenceManager.Instance.AddWorld(id);
            world = ProvenceManager.Instance.worlds[id];
        }

        void Update(){
            world?.eventManager.Raise(new WorldUpdateEvent(world, Time.deltaTime));
        }

        void LateUpate(){
            world?.eventManager.Raise(new WorldLateUpdateEvent(world, Time.deltaTime));
        }

        void FixedUpdate(){
            world?.eventManager.Raise(new WorldFixedUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnGUI(){
            world?.eventManager.Raise(new WorldGUIUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnDrawGizmos(){
            world?.eventManager.Raise(new WorldDrawGizmosUpdateEvent(world, Time.fixedDeltaTime));
        }

        void OnDestroy(){
            world?.eventManager.Raise(new WorldSafetyDestroy(world, Time.fixedDeltaTime));
        }

        void OnApplicationQuit(){
            world?.eventManager.Raise(new WorldSafetyDestroy(world, Time.fixedDeltaTime));
        }

    }
    
}