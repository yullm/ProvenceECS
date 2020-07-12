using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ProvenceECS{
    public class SystemReadyEvent : ProvenceEventArgs{
        public ProvenceSystem system;
        public SystemReadyEvent(ProvenceSystem system){
            this.system = system;
        }
    }

    public class WakeSystemEvent : ProvenceEventArgs{
        public World world;

        public WakeSystemEvent(World world){
            this.world = world;
        }

    }

    public class CacheIntegrityChange : ProvenceEventArgs{
        public Type type;

        public CacheIntegrityChange(Type type){
            this.type = type;
        }
    }

    [System.Serializable]
    public abstract class ProvenceSystem{
        public World world;
        [JsonIgnore]
        [DontDisplayInEditor]
        public bool cacheIsSafe;

        public ProvenceSystem(){
            this.world = null;
            this.cacheIsSafe = false;
        }

        public virtual void Awaken(WakeSystemEvent args){}
        
        public abstract void Initialize(WorldRegistrationComplete args);

        public abstract void GatherCache();

        public abstract void IntegrityCheck(CacheIntegrityChange args);
    }

    [System.Serializable]
    public class SystemManager{
        
        public World world;
        [JsonProperty]
        protected Dictionary<Type,ProvenceSystem> systemDictionary = new Dictionary<Type, ProvenceSystem>();
        protected int readyCount = 0;
        protected int readyTarget = 0;

        public SystemManager(World world){
            this.world = world;  
        }

        public void Initialize(){
            world.eventManager.AddListener<SystemReadyEvent>(SystemReadyCheck);
            foreach(KeyValuePair<Type, ProvenceSystem> kvp in systemDictionary){
                if(kvp.Value == null) continue;
                readyTarget++;
                kvp.Value.world = world;
                world.eventManager.AddListener<WorldRegistrationComplete>(kvp.Value.Initialize);
            }
        }

        protected void SystemReadyCheck(SystemReadyEvent args){
            readyCount++;
            if(readyCount == readyTarget) world.eventManager.Raise<WakeSystemEvent>(new WakeSystemEvent(world));
            if(readyCount > readyTarget) args.system.Awaken(new WakeSystemEvent(world));
        }

        public T AddSystem<T>() where T : ProvenceSystem, new(){
            T system = new T();
            system.world = world;
            systemDictionary[typeof(T)] = system;
            if(system.world != null) system.Initialize(new WorldRegistrationComplete(world));
            return system;
        }

        public T GetSystem<T>() where T : ProvenceSystem{
            if(systemDictionary.ContainsKey(typeof(T))) return (T)systemDictionary[typeof(T)];
            return null;
        }

        public T GetOrAddSystem<T>() where T : ProvenceSystem, new(){
            if(systemDictionary.ContainsKey(typeof(T))) return (T)systemDictionary[typeof(T)];
            else return AddSystem<T>();
        }

        public void RemoveSystem<T>() where T : ProvenceSystem{
            if(systemDictionary.ContainsKey(typeof(T))) systemDictionary.Remove(typeof(T));
        }

        public List<Type> GetCurrentSystemTypes(){
            return systemDictionary.Keys.ToList();
        }

    }
}