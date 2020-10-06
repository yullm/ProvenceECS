using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;
using System.Linq;

namespace ProvenceECS.Mainframe{

    public abstract class ProvenceCollectionEntry{
        public string name;
        public HashSet<string> tags;

        public ProvenceCollectionEntry(){
            this.name = "";
            this.tags = new HashSet<string>();
        }

        public ProvenceCollectionEntry(string name) : this(){
            this.name = name;
        }

    }

    public abstract class ProvenceCollection<T> : Dictionary<string,T> where T : ProvenceCollectionEntry{

        protected string directoryKey;
        [Newtonsoft.Json.JsonIgnore] public bool cacheIsSafe;

        public ProvenceCollection() : base(){
            this.cacheIsSafe = true;
            this.directoryKey = "";
        }

        public ProvenceCollection(Dictionary<string,T> dict) : base(dict){
            this.cacheIsSafe = true;
            this.directoryKey = "";
        }

        public virtual void Save(){
            if(directoryKey.Equals("")) return;
            this.Sort();
            this.cacheIsSafe = false;
            Helpers.SerializeAndSaveToFile(this,
                TableDirectory.GetSubKey(directoryKey, TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey(directoryKey, TableDirectoryKey.FILE),
                TableDirectory.GetSubKey(directoryKey, TableDirectoryKey.EXTENSION)
            );

            ProvenceECS.Mainframe.IO.ConstantCreator constantCreator = new ProvenceECS.Mainframe.IO.ConstantCreator(
                TableDirectory.GetSubKey(directoryKey + "-keys", TableDirectoryKey.FILE), "ProvenceECS.Mainframe", this.Keys.ToArray()
            );
            constantCreator.Save(
                TableDirectory.GetSubKey(directoryKey + "-keys", TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey(directoryKey + "-keys", TableDirectoryKey.FILE) +
                TableDirectory.GetSubKey(directoryKey + "-keys", TableDirectoryKey.EXTENSION)
            );
        }

        public virtual void Backup(){
            if(directoryKey.Equals("")) return;
            Helpers.BackUpFile(TableDirectory.GetSubKey(directoryKey,TableDirectoryKey.DIRECTORY),
                TableDirectory.GetSubKey(directoryKey, TableDirectoryKey.FILE),
                TableDirectory.GetSubKey(directoryKey, TableDirectoryKey.EXTENSION),
            5);
        }

    }

}