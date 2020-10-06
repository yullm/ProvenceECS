using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class SystemPackage : ProvenceCollectionEntry{
        public HashSet<System.Type> systems;

        public SystemPackage(){
            this.systems = new HashSet<System.Type>();
        }
    }

    public class SystemPackageManager : ProvenceCollection<SystemPackage>{

        public SystemPackageManager(){
            this.directoryKey = "system-package-manager";
        }

        public SystemPackageManager(Dictionary<string,SystemPackage> dict) : base(dict){
            this.directoryKey = "system-package-manager";
        }

        public static SystemPackageManager Load(){
            return Helpers.LoadFromSerializedFile<SystemPackageManager>(TableDirectory.GetPath("system-package-manager"));
        }

    }
}