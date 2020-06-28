using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class ActorManualEntry{
        public string name;
        public Actor actorComponent;
        public Dictionary<Type,ProvenceComponent> components;
        public Dictionary<string,List<ProvenceComponent>> categorizedComponents;
        public List<ProvenceComponent> uncategorizedComponents;
        public HashSet<string> tags;

        public ActorManualEntry(){
            this.name = "";
            this.actorComponent = new Actor();
            this.components = new Dictionary<Type,ProvenceComponent>();
            this.categorizedComponents = new Dictionary<string, List<ProvenceComponent>>();
            this.uncategorizedComponents = new List<ProvenceComponent>();
            this.tags = new HashSet<string>();
        }

        public ActorManualEntry(string name) : this(){
            this.name = name;
        }
    }
    [System.Serializable]
    public class ActorManual : Dictionary<string,ActorManualEntry>{

        [Newtonsoft.Json.JsonIgnore] public bool cacheIsSafe;

        public ActorManual() : base(){
            this.cacheIsSafe = true;
        }
        public ActorManual(Dictionary<string,ActorManualEntry> dict) : base(dict){
            this.cacheIsSafe = true;
        }
        
        public static ActorManual Load(){
            ActorManual actorManual = Helpers.LoadFromSerializedFile<ActorManual>(Mainframe.TableDirectory.GetPath("actor-manual"));
            return actorManual;
        }

        public void Save(){
            this.Sort();
            this.cacheIsSafe = false;
            Helpers.SerializeAndSaveToFile(this,TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.FILE),
                TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.EXTENSION)
            );

            IO.ConstantCreator constantCreator = new IO.ConstantCreator(TableDirectory.GetSubKey("actor-manual-keys",TableDirectoryKey.FILE),"ProvenceECS.Mainframe", this.Keys.ToArray());
            constantCreator.Save(
                TableDirectory.GetSubKey("actor-manual-keys",TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey("actor-manual-keys",TableDirectoryKey.FILE) +
                TableDirectory.GetSubKey("actor-manual-keys",TableDirectoryKey.EXTENSION)
            );

        }

        public void Backup(){
            Helpers.BackUpFile(TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.FILE),
                TableDirectory.GetSubKey("actor-manual",TableDirectoryKey.EXTENSION),
            5);
        }

    }

    

}