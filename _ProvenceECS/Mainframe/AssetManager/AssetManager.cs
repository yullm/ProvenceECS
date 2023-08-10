using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProvenceECS.Mainframe.IO;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    //add custom serializer for loading when saved.
    public class ProvenceAsset<T> where T : UnityEngine.Object{

        public string resourcePath;
        [Newtonsoft.Json.JsonIgnore] public T asset;

        public ProvenceAsset(){
            this.resourcePath = "";
            this.asset = null;
        }

        public ProvenceAsset(string resourcePath){
            this.resourcePath = resourcePath;
            asset = Resources.Load<T>(resourcePath);
        }
    }

    public class AssetData : ProvenceCollectionEntry{
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

    public class AssetManager : ProvenceCollection<AssetData>{
       
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

        public override void Save(){
            this.Sort();
            this.cacheIsSafe = false;
            Helpers.SerializeAndSaveToFile(this.addressCache, dataPath + "AssetManager/" , "AssetManager", ".meglo");

            ConstantCreator constantCreator = new ConstantCreator(TypeName() + "Keys", nameSpace, this.Keys.ToArray());
            constantCreator.Save(keyPath, "AssetManagerKeys.cs");
        }

        public override void Backup(){
            Helpers.BackUpFile(dataPath + "AssetManager/" , "AssetManager", ".meglo",5);
        }

        public new static AssetManager Load(){
            AssetManager assetManager = new AssetManager();
            assetManager.addressCache = Helpers.LoadFromSerializedFile<Dictionary<string,AssetData>>(dataPath + "AssetManager/AssetManager.meglo");
            return assetManager;
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
        
        public static void CreateAssetLibrary(){
            HashSet<string> keys = new HashSet<string>();
            foreach(Object resource in Resources.LoadAll("")){
                string name = resource.name;
                if(name.Contains('.')) continue;
                if(resource is Shader shader){
                    int dirIndex = name.LastIndexOf('/') + 1;
                    if(dirIndex > -1) name = name.Substring(dirIndex, name.Length - dirIndex);                    
                }
                keys.Add(name);
            }
            ConstantCreator constantCreator = new ConstantCreator(TypeName() + "Keys", nameSpace, keys.ToArray());
            constantCreator.Save(keyPath, "AssetManagerKeys.cs");
        }
    }

}