using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using ProvenceECS.Mainframe;

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

    public class CacheUpdate<T> : ProvenceEventArgs where T : ProvenceComponent{
        public World world;
        public Dictionary<Entity, ComponentHandle<T>> cache;

        public CacheUpdate(World world, Dictionary<Entity,ComponentHandle<T>> cache){
            this.world = world;
            this.cache = cache;
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

    public class ComponentCache<T> where T : ProvenceComponent{

        public Dictionary<Entity,ComponentHandle<T>> cache;

        public ComponentCache(){
            this.cache = new Dictionary<Entity, ComponentHandle<T>>();
        }

        public ComponentHandle<T> this[Entity key]{
            get => cache[key];
            set => cache[key] = value;
        }

        public Dictionary<Entity,ComponentHandle<T>>.KeyCollection Keys{
            get => cache.Keys;
        }

        public Dictionary<Entity,ComponentHandle<T>>.ValueCollection Values{
            get => cache.Values;
        }

        public int Count{
            get => cache.Count;
        }

        public bool ContainsKey(Entity key){
            return cache.ContainsKey(key);
        }
        
        public void StandardRegistration(World world){
            cache = world.componentManager.GetAllComponentsAsDictionary<T>();
            world.eventManager.AddListener<CacheUpdate<T>>(GatherCache);
        }

        public void InheritedRegistration<J>(World world) where J : ProvenceComponent,T{
            cache = world.componentManager.GetAllComponentsAsDictionary<J>().ToBaseDict<T,J>();
            world.eventManager.AddListener<CacheUpdate<J>>(GatherInheritedCache);
        }

        public void StandardDeregistration(World world){
            world.eventManager.RemoveListener<CacheUpdate<T>>(GatherCache);
        }

        public void InheritedDeregistration<J>(World world) where J : ProvenceComponent,T{
            world.eventManager.RemoveListener<CacheUpdate<J>>(GatherInheritedCache);
        }

        protected void GatherCache(CacheUpdate<T> args){
            cache = args.cache;
        }

        protected void GatherInheritedCache<J>(CacheUpdate<J> args) where J : ProvenceComponent,T{
            cache = args.cache.ToBaseDict<T,J>();
        }

    }

    public enum SystemDestructionType {
        MANAGER_DESTROY = 0,
        SYSTEM_REMOVAL = 1,
        TRANSFER = 2,        
    }

    [System.Serializable]
    public abstract class ProvenceSystem{
        public World world;

        public ProvenceSystem(){
            this.world = null;
        }

        public virtual void Initialize(WorldRegistrationComplete args){
            world.eventManager.AddListener<WakeSystemEvent>(Awaken);      
            world.eventManager.AddListener<WorldSafetyDestroy>(SafetyDestroy);      
            RegisterEventListeners();
            world.eventManager.Raise<SystemReadyEvent>(new SystemReadyEvent(this));
        }

        public virtual void Destroy(SystemDestructionType destructionType){
            world.eventManager.RemoveListener<WakeSystemEvent>(Awaken);
            world.eventManager.RemoveListener<WorldSafetyDestroy>(SafetyDestroy);
            DeregisterEventListeners();
        }

        protected virtual void SafetyDestroy(WorldSafetyDestroy args){}
        
        /// <summary>
        /// Called after Events are registered
        ///</summary>
        public virtual void Awaken(WakeSystemEvent args){}      

        protected abstract void RegisterEventListeners();

        protected abstract void DeregisterEventListeners();
              
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
            AddPackage(ProvenceManager.ProvenceCoreSystemPackage);
            if(Application.isPlaying) AddAllPackages();
            foreach(KeyValuePair<Type, ProvenceSystem> kvp in systemDictionary){
                if(kvp.Value == null) continue;
                readyTarget++;
                kvp.Value.world = world;
                world.eventManager.AddListener<WorldRegistrationComplete>(kvp.Value.Initialize);
            }
        }

        public void Destroy(){
            foreach(ProvenceSystem system in systemDictionary.Values) system.Destroy(SystemDestructionType.MANAGER_DESTROY);
            systemDictionary.Clear();
        }

        protected void AddAllPackages(){
            foreach(string packageName in systemPackages){
                AddPackage(packageName);
            }
        }

        protected void AddPackage(string packageName){
            if(ProvenceManager.Collections<SystemPackage>().ContainsKey(packageName)){
                HashSet<Type> package = ProvenceManager.Collections<SystemPackage>()[packageName].systems;
                foreach(Type systemType in package){
                    if(!systemDictionary.ContainsKey(systemType)){
                        systemDictionary[systemType] = (ProvenceSystem) Activator.CreateInstance(systemType);
                    }
                }
            }            
        }

        public void AddSystemPackageAtRuntime(string packageName){
            if(ProvenceManager.Collections<SystemPackage>().ContainsKey(packageName)){
                HashSet<Type> package = ProvenceManager.Collections<SystemPackage>()[packageName].systems;
                foreach(Type systemType in package){
                    if(!systemDictionary.ContainsKey(systemType)){
                        ProvenceSystem system = (ProvenceSystem) Activator.CreateInstance(systemType);
                        AddSystem((dynamic)system);
                    }
                }
            } 
        }

        protected void AddPackage(HashSet<Type> package){
            foreach(Type systemType in package){
                if(!systemDictionary.ContainsKey(systemType)){
                    systemDictionary[systemType] = (ProvenceSystem) Activator.CreateInstance(systemType);
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
            if(systemDictionary.ContainsKey(typeof(T))) systemDictionary[typeof(T)].Destroy(SystemDestructionType.SYSTEM_REMOVAL);
            T system = new T();
            system.world = world;
            systemDictionary[typeof(T)] = system;
            if(system.world != null) system.Initialize(new WorldRegistrationComplete(world));
            return system;
        }

        public void AddSystem<T>(T system, bool overwrite = true) where T : ProvenceSystem{
            if(systemDictionary.ContainsKey(typeof(T)) && overwrite) systemDictionary[typeof(T)].Destroy(SystemDestructionType.SYSTEM_REMOVAL);
            if(systemDictionary.ContainsKey(typeof(T)) && !overwrite) return;
            system.world = world;
            systemDictionary[typeof(T)] = system;
            if(system.world != null) system.Initialize(new WorldRegistrationComplete(world));
        }

        public T GetSystem<T>() where T : ProvenceSystem{
            if(systemDictionary.ContainsKey(typeof(T))) return (T)systemDictionary[typeof(T)];
            return null;
        }

        public T GetOrAddSystem<T>() where T : ProvenceSystem, new(){
            if(systemDictionary.ContainsKey(typeof(T))) return (T)systemDictionary[typeof(T)];
            else return AddSystem<T>();
        }        

        public void RemoveSystem<T>(SystemDestructionType destructionType = SystemDestructionType.SYSTEM_REMOVAL) where T : ProvenceSystem{
            if(systemDictionary.ContainsKey(typeof(T))){
                systemDictionary[typeof(T)].Destroy(destructionType);
                systemDictionary.Remove(typeof(T));
            }
        }

        public void AddRequiredSystems(HashSet<Type> types){
            foreach(Type systemType in types){
                try{
                    Helpers.InvokeGenericMethod(this,"GetOrAddSystem",systemType);
                }catch(Exception e){
                    Debug.LogWarning(e);
                }
            }
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