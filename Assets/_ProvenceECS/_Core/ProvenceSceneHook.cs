using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceSceneHook : MonoBehaviour{
        public string id;
        public EventManager<ProvenceEventArgs> eventManager;

        void Start(){
            ProvenceManager.Load(id);
        }

        void Update(){
            if(ProvenceManager.Instance.activeWorld != null)
                ProvenceManager.Instance.activeWorld.eventManager.Raise<WorldUpdateEvent>(new WorldUpdateEvent(ProvenceManager.Instance.activeWorld, Time.deltaTime));
        }

        void LateUpate(){
            if(ProvenceManager.Instance.activeWorld != null)
                ProvenceManager.Instance.activeWorld.eventManager.Raise<WorldLateUpdateEvent>(new WorldLateUpdateEvent(ProvenceManager.Instance.activeWorld, Time.deltaTime));
        }

        void FixedUpdate(){
            if(ProvenceManager.Instance.activeWorld != null)
                ProvenceManager.Instance.activeWorld.eventManager.Raise<WorldFixedUpdateEvent>(new WorldFixedUpdateEvent(ProvenceManager.Instance.activeWorld, Time.fixedDeltaTime));
        }

        void OnGUI(){
            if(ProvenceManager.Instance.activeWorld != null)
                ProvenceManager.Instance.activeWorld.eventManager.Raise<WorldGUIUpdateEvent>(new WorldGUIUpdateEvent(ProvenceManager.Instance.activeWorld, Time.fixedDeltaTime));
        }

    }
    
}