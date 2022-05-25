using System.Collections;
using System.Linq;
using System.Collections.Generic;
using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS{


    [System.Serializable]
    public class ProvenceManager{

        public Dictionary<string,World> worlds;
        public string managerID;

        protected static Dictionary<System.Type, IProvenceCollection> collections;
        public static ProvenceCollection<T> Collections<T>() where T : ProvenceCollectionEntry{
            if(collections == null) collections = new Dictionary<System.Type, IProvenceCollection>();
            if(!collections.ContainsKey(typeof(T))) collections[typeof(T)] = ProvenceCollection<T>.Load();            
            return (ProvenceCollection<T>)collections[typeof(T)];
        }

        protected static ProvenceManager instance;
        public static ProvenceManager Instance{
            get{
                if(instance == null) instance = new ProvenceManager();
                return instance;
            }
        }

        /* protected static Mainframe.AssetManager assetManager;
        public static Mainframe.AssetManager AssetManager{
            get{
                if(assetManager == null || !assetManager.cacheIsSafe)
                    assetManager = (AssetManager)Mainframe.AssetManager.Load();
                return assetManager;
            }
        } */

        protected static ModelBank modelBank;
        public static ModelBank ModelBank{
            get{
                if(modelBank == null || !modelBank.cacheIsSafe){
                    modelBank = (ModelBank)ModelBank.Load();
                    modelBank.cacheIsSafe = true;
                }
                return modelBank;
            }
        }

        public static HashSet<System.Type> ProvenceCoreSystemPackage{
            get{
                return new HashSet<System.Type>(){
                    typeof(UnityGameObjectSystem),
                    typeof(CouplingSystem),
                    typeof(ProvenceCameraSystem),
                    typeof(ModelLoaderSystem),
                    typeof(AnimationSystem),
                    typeof(ActorManualSystem),
                    typeof(Network.ProvenceNetworkSystem)
                };
            }
        }
     
        public ProvenceManager(){
            this.worlds = new Dictionary<string, World>();
            this.managerID = System.Guid.NewGuid().ToString();
        }

        public ProvenceManager(string id) : this(){
            this.managerID = id;
        }

        public World AddWorld(string id, ProvenceDelegate<WorldRegistrationComplete> regEvent = null){
            if(string.IsNullOrEmpty(id)) return null;
            World current = Helpers.LoadFromSerializedFile<World>(ProvenceCollection<AssetData>.dataPath + "/Worlds/" + id + ".meglo");
            if(current.id == id){
                AddWorld(current, regEvent);
                return current;
            }
            return null;
        }

        public void AddWorld(World world, ProvenceDelegate<WorldRegistrationComplete> regEvent = null){
            if(!worlds.ContainsKey(world.id)){
                worlds[world.id] = world;
                if(regEvent != null) world.eventManager.AddListener<WorldRegistrationComplete>(regEvent,10);
                if(UnityHelpers.IsCurrentScene(world.worldName)){
                    world.Initialize();
                }else{                    
                    if(UnityHelpers.SceneExists(world.worldName)){
                        //load async then init.
                        UnityHelpers.LoadSceneAsync(world.worldName,()=>{
                            Debug.Log("async load: " + world.worldName);
                            world.Initialize();
                        });  
                    }else{
                        world.Initialize();
                    }
                }
            }
        }

        public void RemoveWorld(World world){
            if(worlds.ContainsKey(world.id)){ 
                worlds.Remove(world.id);
                world.Destroy();
                UnityHelpers.UnloadSceneAsync(world.worldName);
            }
        }

        public void RemoveWorld(string id){
            World world = worlds.Values.Where(w => w.id == id).First();
            if(world != null){ 
                worlds.Remove(id);
                world.Destroy();
                UnityHelpers.UnloadSceneAsync(world.worldName);
            }
        }

        public EntityHandle LookUpEntity(Entity entity){
            foreach(World world in worlds.Values){
                EntityHandle entityHandle = world.LookUpEntity(entity);
                if(entityHandle != null) return entityHandle;
            }
            return null;
        }

        public void BroadcastEvent<T>(T args) where T : ProvenceEventArgs{
            foreach(World world in worlds.Values){
                world.eventManager.Raise<T>(args);                
            }
        }

        public static void Load(string id){
            instance = new ProvenceManager(id);
            World current = Helpers.LoadFromSerializedFile<World>(ProvenceCollection<AssetData>.dataPath + "/Worlds/" + id + ".meglo");
            if(current.id != id) current.id = id;
            instance.AddWorld(current);
        }

        public void Save(){
            if(!managerID.Equals("") && worlds != null){
                foreach(World world in worlds.Values){
                    if(world.id == managerID){
                        world.worldName = UnityHelpers.GetSceneName();
                    }
                    Helpers.SerializeAndSaveToFile<World>(world, ProvenceCollection<AssetData>.dataPath + "/Worlds/", world.id, ".meglo");
                }
            }        
        }

        public  void Backup(){
            foreach(World world in worlds.Values){
                Helpers.BackUpFile(ProvenceCollection<AssetData>.dataPath + "/Worlds/", world.id, ".meglo", 5);
            }
            if(collections == null) return;
            foreach(IProvenceCollection collection in collections.Values){
                collection.Backup();
            }
            ModelBank.Backup();
        }
        
    }
}