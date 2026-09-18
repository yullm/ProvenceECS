using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    
    [DontDisplayInEditor]
    public class Name : ProvenceComponent{
        public string name;

        public Name(){
            this.name = "";
        }

        public Name(string name){
            this.name = name;
        }
    }

}