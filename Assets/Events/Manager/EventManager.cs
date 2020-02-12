using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace ProvenceECS{

    public class EventHandle{
        public World world;
        public ProvenceEventArgs eventArgs;

        public EventHandle(World world, ProvenceEventArgs eventArgs){
            this.world = world;
            this.eventArgs = eventArgs;
        }

        public void Raise(){
            if(world != null && eventArgs!= null){
                var method = typeof(EventManager).GetMethod("Raise");
                var reference = method.MakeGenericMethod(eventArgs.GetType());
                reference.Invoke(world.eventManager,new object[]{eventArgs});
            }
        }
    }

    public abstract class ProvenceEventArgs : ScriptableObject{
        public Entity conditionalEntity;
        public ProvenceEventArgs OnCompleteCallback;
    } //All Events must have their own file to be serialized!

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

        public async Task<T> WaitForEvent<T>() where T : ProvenceEventArgs{
            bool waiting = true;
            T eventArgs = null;
            ProvenceDelegate<T> del = (e) => { 
                eventArgs = e; 
                waiting = false; 
            };
            AddListener<T>(del);
            while(waiting){
                await Task.Delay(1);
            }
            RemoveListener<T>(del);
            return eventArgs;
        }
    
    }
    
}