using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class AnimationEventReciever : MonoBehaviour{

        public Entity entity;
        public World world;

        public void AnimationEvent(string eventData){
            if(world != null && entity != null)
                new AnimationEvent(entity, eventData).Raise(world);
        }

    }

}