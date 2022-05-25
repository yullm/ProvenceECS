using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using Newtonsoft.Json;
using System.Linq;

namespace ProvenceECS.Mainframe{
    public class Model : ProvenceComponent {
        public string manualKey;
        [JsonIgnore] [DontDisplayInManual] public GameObject root;
        [JsonIgnore] [DontDisplayInEditor] public Dictionary<string, int[]> anchors;
        [JsonIgnore] [DontDisplayInEditor] public Animator animatorComponent;
        [JsonIgnore] [DontDisplayInEditor] public string currentState;
        [JsonIgnore] [DontDisplayInEditor] public Dictionary<string, ModelAnimationData> animationData;
        [JsonIgnore] [DontDisplayInEditor] public HashSet<Renderer> renderers;

        public Model(){
            this.manualKey = "";
            this.root = null;
            this.anchors = new Dictionary<string, int[]>();
            this.animatorComponent = null;
            this.currentState = "";
            this.animationData = new Dictionary<string, ModelAnimationData>();
            this.renderers = new HashSet<Renderer>();
        }

        public Model(string key) : this(){
            this.manualKey = key;
        }

    }

    public class ModelAnchorData{
        public string objectName;
        public List<int> hierarchy;

        public ModelAnchorData(){
            this.objectName = "";
            this.hierarchy = new List<int>();
        }

        public ModelAnchorData(string objectName, int[] hierarchy){
            this.objectName = objectName;
            this.hierarchy = new List<int>(hierarchy);
        }
    }

    public class ModelAnimationData{
        public string stateName;
        public int layer;

        public ModelAnimationData(){
            this.stateName = "";
            this.layer = 0;
        }

        public ModelAnimationData(string stateName){
            this.stateName = stateName;
            this.layer = 0;
        }

        public ModelAnimationData(string stateName, int layer){
            this.stateName = stateName;
            this.layer = layer;
        }
    }
    
    public class ModelBankEntry : ProvenceCollectionEntry{

        public string resourcePath;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public Vector3 scaleOffset;
        public Dictionary<string, ModelAnchorData> anchors;
        public Dictionary<string, ModelAnimationData> animationData;
        
        public ModelBankEntry() : base(){
            this.resourcePath = "";
            this.positionOffset = new Vector3();
            this.rotationOffset = new Vector3();
            this.scaleOffset = Vector3.one;
            this.anchors = new Dictionary<string, ModelAnchorData>();
            this.animationData = new Dictionary<string, ModelAnimationData>();
        }

        public ModelBankEntry(string name) : this(){
            this.name = name;
        }

    }

    public class ModelBank : ProvenceCollection<ModelBankEntry>{

        [JsonIgnore] public Dictionary<string,GameObject> assetCache;

        public ModelBank() : base(){
            this.assetCache = new Dictionary<string, GameObject>();
        }

        public ModelBank(Dictionary<string,ModelBankEntry> dict) : base(dict){
            this.assetCache = new Dictionary<string, GameObject>();
        }

        public void SetResourcePath(string key, string path){
            if(this.ContainsKey(key)){
                string resourcePath = ProvenceECS.Mainframe.AssetManager.ParseResourceFromPath(path);
                this[key].resourcePath = resourcePath;
                assetCache.Remove(key);
            }
        }

        public GameObject LoadModel(string key){
            if(this.ContainsKey(key)){
                if(assetCache.ContainsKey(key)) return assetCache[key];
                else{
                    if(this[key].resourcePath != null && !this[key].resourcePath.Equals("")){
                        GameObject model = Resources.Load<GameObject>(this[key].resourcePath);
                        if(model != null){
                            assetCache[key] = model;
                            return model;
                        }
                    }
                }
            }
            return null;   
        }
        
        public void LoadModel(ComponentHandle<Model> modelHandle){
            if(this.ContainsKey(modelHandle.component.manualKey)){
                ModelBankEntry entry = this[modelHandle.component.manualKey];
                
                GameObject entityObj = modelHandle.world.GetOrCreateComponent<UnityGameObject>(modelHandle.entity).component.gameObject;
                entityObj.Clear();

                GameObject asset = this.LoadModel(entry.name);
                if(asset != null){
                    modelHandle.component.root = Object.Instantiate(asset,entityObj.transform.position + entry.positionOffset, entityObj.transform.rotation, entityObj.transform);
                    modelHandle.component.root.transform.Rotate(entry.rotationOffset);
                    Vector3 localScale = modelHandle.component.root.transform.localScale;
                    modelHandle.component.root.transform.localScale = Vector3.Scale(localScale,entry.scaleOffset);

                    foreach(KeyValuePair<string, ModelAnchorData> kvp in entry.anchors){
                        modelHandle.component.anchors[kvp.Key] = kvp.Value.hierarchy.ToArray();
                        GameObject anchor = GetChildByHierarhcy(modelHandle.component.root, kvp.Value.hierarchy.ToArray());
                        //if(anchor != null) modelHandle.component.anchors[kvp.Key] = anchor;
                    }

                    Animator animatorComponent = modelHandle.component.root.GetComponent<Animator>();
                    if(animatorComponent != null){
                        modelHandle.component.animatorComponent = animatorComponent;
                        modelHandle.component.animationData = entry.animationData;

                        AnimationEventReciever reciever = modelHandle.component.root.AddComponent<AnimationEventReciever>();
                        reciever.entity = modelHandle.entity;
                        reciever.world = modelHandle.world;
                    }

                    modelHandle.component.renderers = modelHandle.component.root.GetComponentsInChildren<Renderer>().ToSet();
                    
                }else Debug.LogWarning("Model Asset Missing, key: " + modelHandle.component.manualKey);
            }else Debug.LogWarning("Model Entry Missing, key: " + modelHandle.component.manualKey);
        }

        public static GameObject GetChildByHierarhcy(GameObject root, int[] hierarchy){
            GameObject current = root;
            foreach(int i in hierarchy){
                try{
                    current = current.transform.GetChild(i).gameObject;
                }catch(System.Exception e){
                    Debug.LogWarning(e);
                    return current;
                }
            }
            return current;
        }
        
    }
}