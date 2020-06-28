using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProvenceECS;

namespace ProvenceECS.Mainframe{

    public class Model : ProvenceComponent {
        public string address;
        [Newtonsoft.Json.JsonIgnore] [DontDisplayInManual] public GameObject root;

        public Model(){
            this.address = "";
        }

        public Model(string address){
            this.address = address;
        }

    }

}