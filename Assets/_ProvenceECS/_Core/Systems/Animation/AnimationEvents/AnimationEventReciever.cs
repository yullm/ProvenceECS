using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class AnimationEventReciever : MonoBehaviour{

        public Entity entity;
        public World world;

        public void AnimationEvent(string eventName){
            if(world != null && entity != null)
                world.eventManager.Raise<AnimationEvent>(new AnimationEvent(entity, eventName));
        }

    }

}