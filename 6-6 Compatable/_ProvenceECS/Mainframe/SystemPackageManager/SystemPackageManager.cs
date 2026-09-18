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

}