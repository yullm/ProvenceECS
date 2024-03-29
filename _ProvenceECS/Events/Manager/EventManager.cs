﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    public delegate void EmptyDelegate();
    public delegate void ProvenceDelegate<T>(T e);
    public delegate J ProvenceReturnDelegate<T,J>(T e);

    public abstract class ProvenceEventArgs{

        public byte senderID;

        public ProvenceEventArgs(){
            this.senderID = 0;
        }

    }

    public class EventManager<ARGTYPE>{

        public World world;

        protected Dictionary<int, Dictionary<Type, Delegate>> delegates;
        protected Dictionary<string, Dictionary<int, Dictionary<Type,Delegate>>> conditionalDelegates;
        protected Dictionary<Type, Dictionary<Type, Delegate>> returnDelegates;

        public EventManager(){
            this.delegates = new Dictionary<int, Dictionary<Type, Delegate>>();
            this.conditionalDelegates = new Dictionary<string, Dictionary<int, Dictionary<Type, Delegate>>>();
            this.returnDelegates = new Dictionary<Type, Dictionary<Type, Delegate>>();
        }

        public EventManager(World world) : this(){
            this.world = world;
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

        public void Clear<T>() where T : ARGTYPE{
            foreach(Dictionary<Type,Delegate> dict in delegates.Values){
                dict.Remove(typeof(T));
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

        public void AddListener<T>(Entity condition, ProvenceDelegate<T> del, int stage = 0) where T : ARGTYPE{
            if(condition == null){
                Debug.Log("Missing Condition");
                return;
            }
            if(!conditionalDelegates.ContainsKey(condition)) conditionalDelegates[condition] = new Dictionary<int, Dictionary<Type, Delegate>>();
            if(!conditionalDelegates[condition].ContainsKey(stage)) conditionalDelegates[condition][stage] = new Dictionary<Type, Delegate>();
            if(conditionalDelegates[condition][stage].ContainsKey(typeof(T))){
                Delegate tempDel = conditionalDelegates[condition][stage][typeof(T)];
                conditionalDelegates[condition][stage][typeof(T)] = Delegate.Combine(tempDel,del);
            }else{
               conditionalDelegates[condition][stage][typeof(T)] = del;
            }
            conditionalDelegates[condition] = conditionalDelegates[condition].OrderBy(d => d.Key).ToDictionary(d => d.Key,d => d.Value);
        }

        public void RemoveListener<T>(Entity condition, ProvenceDelegate<T> del) where T : ARGTYPE{
            if(!conditionalDelegates.ContainsKey(condition)) return;
            foreach(Dictionary<Type,Delegate> dict in conditionalDelegates[condition].Values){
                if(dict.ContainsKey(typeof(T))){
                    var currentDel = Delegate.Remove(dict[typeof(T)], del);
                    if(currentDel == null) dict.Remove(typeof(T));
                    else dict[typeof(T)] = currentDel;
                }
            }
        }

        public void Raise<T>(Entity condition, T args){
            if(!conditionalDelegates.ContainsKey(condition)){
                Debug.LogWarning("No registed events of type: " + typeof(T) + " for: " + condition);
                return;
            }
            if(args == null) {
                Debug.LogWarning("Invalid event argument: " + args.GetType().ToString());
                return;
            }
            Dictionary<int,Dictionary<Type,Delegate>> evaluatingDict = new Dictionary<int, Dictionary<Type, Delegate>>(conditionalDelegates[condition]);
            foreach(KeyValuePair<int,Dictionary<Type, Delegate>> kvp in evaluatingDict){
                if(!conditionalDelegates[condition].ContainsKey(kvp.Key)) continue;
                if(kvp.Value.ContainsKey(typeof(T))){                         
                    if(conditionalDelegates[condition][kvp.Key].ContainsKey(typeof(T)))
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

        public void ClearListeners(){
            delegates.Clear();
            conditionalDelegates.Clear();
            returnDelegates.Clear();
        }
        
    }
}