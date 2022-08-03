using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System.Linq;
using ProvenceECS.Mainframe.IO;
using ProvenceECS.Network;

namespace ProvenceECS.Mainframe{

    [ProvencePacket(10)]
    public class SetEntityToManualEntry<T> : ProvenceEventArgs where T : ProvenceCollectionEntry{
        
        public Entity entity;
        public string key;
        public Vector3? position;
        public Vector3? rotation;

        public SetEntityToManualEntry(){
            this.entity = null;
            this.key = null;
            this.position = null;
            this.rotation = null;
        }

        public SetEntityToManualEntry(Entity entity) : this(){
            this.entity = entity;
        }

        public SetEntityToManualEntry(Entity entity, string key, Vector3? position = null, Vector3? rotation = null){
            this.entity = entity;
            this.key = key;
            this.position = position;
            this.rotation = rotation;
        }
        

    }

    [ProvencePacket(11)]
    public class CreateEntryInstance<T> : ProvenceEventArgs where T : ProvenceCollectionEntry{
        
        public string key;
        public Vector3? position;
        public Vector3? rotation;

        public CreateEntryInstance(){
            this.key = null;
            this.position = null;
            this.rotation = null;
        }

        public CreateEntryInstance(string key, Vector3? position = null, Vector3? rotation = null){
            this.key = key;
            this.position = position;
            this.rotation = rotation;
        }

    }

    public abstract class MainframeTag : ProvenceComponent{
        public HashSet<System.Type> requiredComponents;

        public MainframeTag(){
            this.requiredComponents = new HashSet<System.Type>();
        }
    }

    public abstract class ProvenceCollectionEntry{
        public string name;
        public Dictionary<System.Type,ProvenceComponent> components;
        public HashSet<System.Type> tags;

        public ProvenceCollectionEntry(){
            this.name = "";
            this.components = new Dictionary<System.Type, ProvenceComponent>();
            this.tags = new HashSet<System.Type>();
        }

        public ProvenceCollectionEntry(string name) : this(){
            this.name = name;
        }

    }

    public class ProvenceCollectionInstance<T> : ProvenceComponent where T : ProvenceCollectionEntry{
        public string key;

        public ProvenceCollectionInstance(){
            this.key = "";
        }

        public ProvenceCollectionInstance(string key){
            this.key = key;
        }
    }

    public interface IProvenceCollection{
        void Save();
        void Backup();
    }

    public class ProvenceCollection<T> : Dictionary<string,T>, IProvenceCollection where T : ProvenceCollectionEntry{
        public static readonly string nameSpace = @"ProvenceECS.Mainframe";
        public static readonly string dataPath = @"./_ProvenceECSData/Mainframe/";
        public static readonly string keyPath = @"./Assets/_ProvenceMainframe/";

        [Newtonsoft.Json.JsonIgnore] public bool cacheIsSafe;

        public ProvenceCollection() : base(){
            this.cacheIsSafe = true;            
        }

        public ProvenceCollection(Dictionary<string,T> dict) : base(dict){
            this.cacheIsSafe = true;
        }

        protected virtual EntityHandle CreateInstance(World world, string key, Vector3? position = null, Vector3? rotation = null){ // add position and rotation
            if(this.ContainsKey(key) && world != null){
                EntityHandle entityHandle = world.CreateEntity();
                //add missing tag comps if any
                GameObject gameObject = entityHandle.GetOrCreateComponent<UnityGameObject>().component.gameObject;
                if(position != null) gameObject.transform.position = (Vector3)position;
                if(rotation != null) gameObject.transform.rotation = Quaternion.Euler((Vector3)rotation);
                entityHandle.AddComponentSet(this[key].components.Values.ToSet().Clone());
                entityHandle.AddComponent<ProvenceCollectionInstance<T>>(new ProvenceCollectionInstance<T>(key));
                foreach(System.Type tag in this[key].tags) entityHandle.AddComponent((dynamic)System.Activator.CreateInstance(tag));
                return entityHandle;
            }
            return null;
        }

        public virtual EntityHandle CreateEntryInstance(World world, string key, Vector3? position = null, Vector3? rotation = null){
            return CreateInstance(world, key, position, rotation);
        }

        public virtual EntityHandle CreateEntryInstance<U>(World world, string key, Vector3? position = null, Vector3? rotation = null) where U : MainframeTag, new(){
            if(this.ContainsKey(key))
                foreach(System.Type tag in this[key].tags) 
                    if(tag == typeof(U))
                        return CreateInstance(world, key, position, rotation);
            return null;
        }

        public virtual void SetEntityToEntry(World world, Entity entity, string key, Vector3? position = null, Vector3? rotation = null){
            if(world.LookUpEntity(entity) == null) world.CreateEntity(entity);
            if(this.ContainsKey(key)){
                GameObject gameObject = world.GetOrCreateComponent<UnityGameObject>(entity).component.gameObject;
                if(position != null) gameObject.transform.position = (Vector3)position;
                if(rotation != null) gameObject.transform.rotation = Quaternion.Euler((Vector3)rotation);
                world.AddComponentSet(entity, this[key].components.Values.ToSet().Clone());
                world.AddComponent<ProvenceCollectionInstance<T>>(entity, new ProvenceCollectionInstance<T>(key));
                foreach(System.Type tag in this[key].tags) world.AddComponent(entity, (dynamic)System.Activator.CreateInstance(tag));
            }
        }

        public virtual void Save(){
            this.Sort();
            this.cacheIsSafe = false;
            Helpers.SerializeAndSaveToFile(this, dataPath + TypeName() + "/", TypeName(), ".meglo");

            ConstantCreator constantCreator = new ConstantCreator(TypeName() + "Keys", nameSpace, this.Keys.ToArray());
            constantCreator.Save(keyPath, TypeName() + "Keys.cs");
        }

        public virtual void Backup(){
            Helpers.BackUpFile(dataPath + TypeName() + "/", TypeName(), ".meglo", 5);
        }

        public static ProvenceCollection<T> Load(){
            return Helpers.LoadFromSerializedFile<ProvenceCollection<T>>(dataPath + TypeName() + "/" + TypeName() + ".meglo");
        }

        protected static string TypeName(){
            return typeof(T).Name.Replace("Entry","");
        }

    }

}