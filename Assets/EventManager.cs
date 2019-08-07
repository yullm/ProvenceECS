using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class EventHandle<T>{
        public event System.EventHandler<T> ev;
        public void Raise(Object sender, T args){
            if(ev != null)ev.Invoke(sender, args);
        } 
    }

    public class EventManager : MonoBehaviour
    {
        public World world;
        public EventHandle<World> worldUpdate = new EventHandle<World>();
    
    }
     
}