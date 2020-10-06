using System.Collections;
using System.Collections.Generic;
using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS{
    [System.Serializable]
    public class ProvenceManager{

        public List<World> worlds;
        public World activeWorld;
        public string managerID;

        protected static ProvenceManager instance;
        public static ProvenceManager Instance{
            get{
                if(instance == null) instance = new ProvenceManager();
                return instance;
            }
        }

        protected static Mainframe.AssetManager assetManager;
        public static Mainframe.AssetManager AssetManager{
            get{
                if(assetManager == null || !assetManager.cacheIsSafe)
                    assetManager = Mainframe.AssetManager.Load();
                return assetManager;
            }
        }

        protected static Mainframe.SystemPackageManager systemPackageManager;
        public static Mainframe.SystemPackageManager SystemPackageManager{
            get{
                if(systemPackageManager == null || !systemPackageManager.cacheIsSafe)
                    systemPackageManager = Mainframe.SystemPackageManager.Load();
                return systemPackageManager;
            }
        }
     
        public ProvenceManager(){
            this.worlds = new List<World>();
            this.managerID = System.Guid.NewGuid().ToString();
        }

        public ProvenceManager(string id){
            this.worlds = new List<World>();
            this.managerID = id;
        }

        public World AddWorld(World world){
            if(!worlds.Contains(world)) worlds.Add(world);
            if(worlds.Count == 1) activeWorld = world;
            return world;
        }

        public  void RemoveWorld(World world){
            if(worlds.Contains(world)) worlds.Remove(world);
            world.gameObjectManager.Clear();
            if(activeWorld == world){ 
                if(worlds.Count > 0) activeWorld = worlds[0];
                else activeWorld = null;
            }
        }

        public EntityHandle LookUpEntity(Entity entity){
            foreach(World world in worlds){
                EntityHandle entityHandle = world.LookUpEntity(entity);
                if(entityHandle != null) return entityHandle;
            }
            return null;
        }

        public void BroadcastEvent<T>(T args) where T : ProvenceEventArgs{
            foreach(World world in worlds){
                world.eventManager.Raise<T>(args);                
            }
        }

        public static void Load(string id){
            ProvenceManager current = null;
            current = Helpers.LoadFromSerializedFile<ProvenceManager>(Mainframe.TableDirectory.GetSubKey("worlds", Mainframe.TableDirectoryKey.DIRECTORY) + id + ".meglo");
            if(current.managerID != id) current = new ProvenceManager(id);        
            foreach(World world in current.worlds) world.Initialize();
            ProvenceManager.instance = current;
        }

        public void Save(){
            if(!managerID.Equals("") && worlds != null){
                foreach(World world in worlds) world.Organize();
                Helpers.SerializeAndSaveToFile<ProvenceManager>(this, Mainframe.TableDirectory.GetSubKey("worlds", Mainframe.TableDirectoryKey.DIRECTORY), managerID, ".meglo");
            }        
        }

        public  void Backup(){
            Helpers.BackUpFile(TableDirectory.GetSubKey("worlds",TableDirectoryKey.DIRECTORY),managerID,".meglo",5);
            AssetManager.Backup();
            Ransacked.Mainframe.RansackedMainframe.Backup();
        }
        
    }
}