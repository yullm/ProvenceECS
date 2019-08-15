using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceEventArgs{}

    public delegate void ProvenceDelegate<T> (T e) where T : ProvenceEventArgs;
    public class EventManager : MonoBehaviour
    {
        public World world;
        private Dictionary<System.Type,System.Delegate> delegates = new Dictionary<System.Type, System.Delegate>();

        public void AddListener<T> (ProvenceDelegate<T> del) where T : ProvenceEventArgs{
            if(delegates.ContainsKey(typeof(T))){
                System.Delegate tempDel = delegates[typeof(T)];
                delegates[typeof(T)] = System.Delegate.Combine(tempDel,del);
            }else{
                delegates[typeof(T)] = del;
            }
        }

        public void RemoveListener<T> (ProvenceDelegate<T> del) where T : ProvenceEventArgs{
            if(delegates.ContainsKey(typeof(T))){
                var currentDel = System.Delegate.Remove(delegates[typeof(T)], del);
                if(currentDel == null) delegates.Remove(typeof(T));
                else delegates[typeof(T)] = currentDel;
            }
        }

        public void Raise<T>(T args) where T : ProvenceEventArgs{
            if(args == null) {
                Debug.Log("Invalid event argument: " + args.GetType().ToString());
                return;
            }
            if(delegates.ContainsKey(typeof(T))) delegates[typeof(T)].DynamicInvoke(args);
        }
    
    }
    
}