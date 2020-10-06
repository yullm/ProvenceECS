using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class AssetData{
        public string address;
        public string resourcePath;

        public AssetData(){
            this.address = "";
            this.resourcePath = "";
        }

        public AssetData(string address, string resourcePath){
            this.address = address;
            this.resourcePath = resourcePath;
        }
    }

    public class AssetManager{

        [Newtonsoft.Json.JsonIgnore] public bool cacheIsSafe;        
        public Dictionary<string,AssetData> addressCache; //nickname,Resource path : not actual file path
        protected Dictionary<string,Object> assetCache;

        public AssetManager(){
            this.cacheIsSafe = true;
            this.addressCache = new Dictionary<string, AssetData>();
            this.assetCache = new Dictionary<string, Object>();
        }

        public void AddPair(string key, string value){
            addressCache[key] = new AssetData(key,value);
        }

        public void ChangeKey(string oldKey, string newKey){
            if(addressCache.ContainsKey(oldKey) && !oldKey.Equals(newKey)){
                addressCache[newKey] = addressCache[oldKey];
                addressCache.Remove(oldKey);
            }
        }

        public void SetResourceByPath(string key, string path){
            string resourcePath = ParseResourceFromPath(path);
            addressCache[key].resourcePath = resourcePath;
        }

        public static AssetManager Load(){
            AssetManager assetManager = new AssetManager();
            assetManager.addressCache = Helpers.LoadFromSerializedFile<Dictionary<string,AssetData>>(Mainframe.TableDirectory.GetPath("asset-manager"));
            return assetManager;
        }

        public void Backup(){
            Helpers.BackUpFile(
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.FILE),
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.EXTENSION),
            5);
        }

        public void Save(){
            addressCache.Sort();
            cacheIsSafe = false;
            Helpers.SerializeAndSaveToFile(addressCache,
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.DIRECTORY), 
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.FILE),
                TableDirectory.GetSubKey("asset-manager",TableDirectoryKey.EXTENSION)
            );
        }

        public T LoadAsset<T>(string address) where T : Object{
            try{
                if(assetCache.ContainsKey(address)){
                    return (T)assetCache[address];
                }
                if(addressCache.ContainsKey(address)){
                    T asset = (T)Resources.LoadAsync(addressCache[address].resourcePath).asset;
                    assetCache[address] = asset;
                    return asset;
                }else{
                    return null;
                }
            }catch(System.Exception e){
                Debug.LogWarning(e);
                return null;
            }
        }

        public static string ParseResourceFromPath(string path){
            if(path.Contains(@"/Resources/")){
                string[] pathParts = path.Split(new string[]{"/Resources/"}, System.StringSplitOptions.RemoveEmptyEntries);
                if(pathParts.Length > 1){
                    string[] resourceParts = pathParts[1].Split('.');
                    return resourceParts[0];
                }
            }
            return "";
        }
    }

}