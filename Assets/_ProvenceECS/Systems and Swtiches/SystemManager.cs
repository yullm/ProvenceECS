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

    public class CacheIntegrityChange<T> : ProvenceEventArgs{
        public World world;

        public CacheIntegrityChange(World world){
            this.world = world;
        }
    }

    public class PlayStart : ProvenceEventArgs{
        public World world;
        public float time;
        
        public PlayStart(World world, float time){
            this.world = world;
            this.time = time;
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

        public virtual void Initialize(WorldRegistrationComplete args){
            RegisterEventListeners();
            world.eventManager.Raise<SystemReadyEvent>(new SystemReadyEvent(this));
        }

        public virtual void Awaken(WakeSystemEvent args){}      

        protected abstract void RegisterEventListeners();

        protected abstract void GatherCache();

        protected virtual void IntegrityCheck<T>(CacheIntegrityChange<T> args){
            cacheIsSafe = false;
        }

    }

    [System.Serializable]
    public class SystemManager{
        
        public World world;
        [JsonProperty]
        protected Dictionary<Type,ProvenceSystem> systemDictionary;
        public HashSet<string> systemPackages;
        protected int readyCount = 0;
        protected int readyTarget = 0;
        protected float playStartTimer = 0f;
        protected float playStartGoal = 0.1f;

        public SystemManager(World world){
            this.systemDictionary = new Dictionary<Type, ProvenceSystem>();
            this.systemPackages = new HashSet<string>();
            this.world = world;  
        }

        public void Initialize(){
            world.eventManager.AddListener<SystemReadyEvent>(SystemReadyCheck);
            if(Application.isPlaying) AddPackageSystems();
            foreach(KeyValuePair<Type, ProvenceSystem> kvp in systemDictionary){
                if(kvp.Value == null) continue;
                readyTarget++;
                kvp.Value.world = world;
                world.eventManager.AddListener<WorldRegistrationComplete>(kvp.Value.Initialize);
            }
        }

        protected void AddPackageSystems(){
            foreach(string packageName in systemPackages){
                if(ProvenceManager.SystemPackageManager.ContainsKey(packageName)){
                    HashSet<Type> package = ProvenceManager.SystemPackageManager[packageName].systems;
                    foreach(Type systemType in package){
                        if(!systemDictionary.ContainsKey(systemType)){
                            systemDictionary[systemType] = (ProvenceSystem) Activator.CreateInstance(systemType);
                        }
                    }
                }
            }
        }

        protected void SystemReadyCheck(SystemReadyEvent args){
            readyCount++;
            if(readyCount > readyTarget){
                args.system.Awaken(new WakeSystemEvent(world));
                return;
            }
            if(readyCount == readyTarget){
                world.eventManager.Raise<WakeSystemEvent>(new WakeSystemEvent(world));
                world.eventManager.AddListener<WorldUpdateEvent>(WaitForPlayStart);
            }
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

        protected void WaitForPlayStart(WorldUpdateEvent args){
            if(playStartTimer < playStartGoal){
                playStartTimer += Time.deltaTime;
                return;
            }
            world.eventManager.RemoveListener<WorldUpdateEvent>(WaitForPlayStart);
            world.eventManager.Raise<PlayStart>(new PlayStart(world, Time.time));
        }

    }
}