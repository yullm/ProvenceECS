using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public delegate void ProvenceDelegate<T>(T e);
    public delegate J ProvenceReturnDelegate<T,J>(T e);
    public delegate T ProvenceStackDelegate<T>(T e);

    public abstract class ProvenceEventArgs{}

    public class EventManager<ARGTYPE>{

        public World world;

        protected Dictionary<int, Dictionary<Type, Delegate>> delegates;
        protected Dictionary<Type, Dictionary<Type, Delegate>> returnDelegates;
        protected Dictionary<object, Dictionary<int, Dictionary<Type, HashSet<Delegate>>>> stackDelegates;

        public EventManager(){
            this.delegates = new Dictionary<int, Dictionary<Type, Delegate>>();
            this.returnDelegates = new Dictionary<Type, Dictionary<Type, Delegate>>();
            this.stackDelegates = new Dictionary<object, Dictionary<int, Dictionary<Type, HashSet<Delegate>>>>();
        }

        public EventManager(World world){
            this.world = world;
            this.delegates = new Dictionary<int, Dictionary<Type, Delegate>>();
            this.returnDelegates = new Dictionary<Type, Dictionary<Type, Delegate>>();
            this.stackDelegates = new Dictionary<object, Dictionary<int, Dictionary<Type, HashSet<Delegate>>>>();
        }

        public void AddListener<T> (ProvenceDelegate<T> del, int stage = 0) where T : ARGTYPE{
            if(!delegates.ContainsKey(stage)) delegates[stage] = new Dictionary<Type, Delegate>();
            if(delegates[stage].ContainsKey(typeof(T))){
                Delegate tempDel = delegates[stage][typeof(T)];
                delegates[stage][typeof(T)] = Delegate.Combine(tempDel,del);
            }else{
                delegates[stage][typeof(T)] = del;
            }
            delegates = delegates.OrderBy(d => d.Key).ToDictionary(d => d.Key,d => d.Value);
        }

        public void RemoveListener<T> (ProvenceDelegate<T> del) where T : ARGTYPE{
            foreach(Dictionary<Type,Delegate> dict in delegates.Values){
                if(dict.ContainsKey(typeof(T))){
                    var currentDel = Delegate.Remove(dict[typeof(T)], del);
                    if(currentDel == null) dict.Remove(typeof(T));
                    else dict[typeof(T)] = currentDel;
                }
            }
        }

        public void Raise<T>(T args) where T : ARGTYPE{
            if(args == null) {
                Debug.Log("Invalid event argument: " + args.GetType().ToString());
                return;
            }
            Dictionary<int,Dictionary<Type,Delegate>> evaluatingDict = new Dictionary<int, Dictionary<Type, Delegate>>(delegates);
            foreach(KeyValuePair<int,Dictionary<Type, Delegate>> kvp in evaluatingDict){
                if(!delegates.ContainsKey(kvp.Key)) continue;
                if(kvp.Value.ContainsKey(typeof(T))){                         
                    if(delegates[kvp.Key].ContainsKey(typeof(T)))
                        ((ProvenceDelegate<T>) kvp.Value[typeof(T)]).Invoke(args);
                }
            }              
        }

        public void RegisterReturnMethod<T,RETTYPE>(ProvenceReturnDelegate<T,RETTYPE> del) where T : ARGTYPE{
            if(!returnDelegates.ContainsKey(typeof(T))) returnDelegates[typeof(T)] = new Dictionary<Type, Delegate>();
            returnDelegates[typeof(T)][typeof(RETTYPE)] = del;
        }

        public void UnregisterReturnMethod<T,RETTYPE>(ProvenceReturnDelegate<T,RETTYPE> del) where T : ARGTYPE{
            if(returnDelegates.ContainsKey(typeof(T)) && returnDelegates[typeof(T)].ContainsKey(typeof(RETTYPE))){
                var currentDel = Delegate.Remove(returnDelegates[typeof(T)][typeof(RETTYPE)], del);
                if(currentDel == null) returnDelegates[typeof(T)].Remove(typeof(RETTYPE));
                if(returnDelegates[typeof(T)].Count == 0) returnDelegates.Remove(typeof(T));
            }
        }

        public RETTYPE RaiseReturn<T,RETTYPE>(T args) where T : ARGTYPE{
            if(returnDelegates.ContainsKey(typeof(T)) && returnDelegates[typeof(T)].ContainsKey(typeof(RETTYPE))) 
                return (RETTYPE) ((ProvenceReturnDelegate<T,RETTYPE>) returnDelegates[typeof(T)][typeof(RETTYPE)]).Invoke(args);
            throw new MissingReferenceException("Return Method Missing");
        }

        public void RegisterStackMethod<T>(ProvenceStackDelegate<T> del, int stage = 0, object condition = null) where T : ARGTYPE{
            if(condition == null) condition = "empty-condition";
            if(!stackDelegates.ContainsKey(condition)) stackDelegates[condition] = new Dictionary<int, Dictionary<Type, HashSet<Delegate>>>();
            if(!stackDelegates[condition].ContainsKey(stage)) stackDelegates[condition][stage] = new Dictionary<Type, HashSet<Delegate>>();
            if(!stackDelegates[condition][stage].ContainsKey(typeof(T))) stackDelegates[condition][stage][typeof(T)] = new HashSet<Delegate>();
            stackDelegates[condition][stage][typeof(T)].Add(del);
            stackDelegates[condition] = stackDelegates[condition].OrderBy(d => d.Key).ToDictionary(d => d.Key,d => d.Value);
        }

        public void UnregisterStackMethod<T>(ProvenceStackDelegate<T> del, object condition = null) where T : ARGTYPE{
            if(condition == null) condition = "empty-condition";
                if(stackDelegates.ContainsKey(condition)){
                foreach(Dictionary<Type,HashSet<Delegate>> dict in stackDelegates[condition].Values){
                    if(dict.ContainsKey(typeof(T))){
                        dict[typeof(T)].Remove(del);
                    }
                }
            }
        }

        public T RaiseStack<T>(T args, object condition = null) where T : ARGTYPE{
            if(condition == null) condition = "empty-condition";
            T stackItem = args;
            if(!stackDelegates.ContainsKey(condition)) return args;
            Dictionary<int,Dictionary<Type, HashSet<Delegate>>> evaluatingDict = new Dictionary<int, Dictionary<Type, HashSet<Delegate>>>(stackDelegates[condition]);
            foreach(KeyValuePair<int, Dictionary<Type, HashSet<Delegate>>> kvp in evaluatingDict){
                if(!stackDelegates[condition].ContainsKey(kvp.Key)) continue;
                if(kvp.Value.ContainsKey(typeof(T))){                         
                    if(stackDelegates[condition][kvp.Key].ContainsKey(typeof(T))){
                        foreach (Delegate method in evaluatingDict[kvp.Key][typeof(T)]){
                            stackItem = (T)((ProvenceStackDelegate<T>)method).Invoke(stackItem);
                        }                        
                    }
                }
            }
            return stackItem;
        }

        public void ClearListeners(){
            delegates.Clear();
        }
        
    }
}