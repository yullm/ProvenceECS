using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public delegate void ProvenceDelegate<T> (T e);
    //public delegate T ProvenceSameReturnDelegate<T>(T e);

    public abstract class ProvenceEventArgs{
        public Entity conditionalEntity;
        public ProvenceEventArgs OnCompleteCallback;
    }

    public class EventManager<J>{

        public World world;

        private Dictionary<int,Dictionary<Type,Delegate>> delegates;

            public EventManager(){
                delegates = new Dictionary<int, Dictionary<Type, Delegate>>();
            }

            public EventManager(World world){
                this.world = world;
                delegates = new Dictionary<int, Dictionary<Type, Delegate>>();
            }

            public void AddListener<T> (ProvenceDelegate<T> del, int stage = 0) where T : J{
                if(!delegates.ContainsKey(stage)) delegates[stage] = new Dictionary<Type, Delegate>();
                if(delegates[stage].ContainsKey(typeof(T))){
                    Delegate tempDel = delegates[stage][typeof(T)];
                    delegates[stage][typeof(T)] = Delegate.Combine(tempDel,del);
                }else{
                    delegates[stage][typeof(T)] = del;
                }
                delegates = delegates.OrderBy(d => d.Key).ToDictionary(d => d.Key,d => d.Value);
            }

            public void RemoveListener<T> (ProvenceDelegate<T> del) where T : J{
                foreach(Dictionary<Type,Delegate> dict in delegates.Values){
                    if(dict.ContainsKey(typeof(T))){
                        var currentDel = Delegate.Remove(dict[typeof(T)], del);
                        if(currentDel == null) dict.Remove(typeof(T));
                        else dict[typeof(T)] = currentDel;
                    }
                }
            }

            public void Raise<T>(T args) where T : J{
                if(args == null) {
                    Debug.Log("Invalid event argument: " + args.GetType().ToString());
                    return;
                }
                foreach(Dictionary<Type,Delegate> dict in delegates.Values){
                    if(dict.ContainsKey(typeof(T))) dict[typeof(T)].DynamicInvoke(args);
                }                
            }

            public void ClearListeners(){
                delegates.Clear();
            }
        
    }
}