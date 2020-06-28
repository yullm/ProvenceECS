using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{

    public class Position : ProvenceComponent{


        public Vector3 worldPosition;
        public Vector3 screenPosition;
        public Vector3 viewportPosition;

        public Position(){
            worldPosition = new Vector3();
            screenPosition = new Vector3();
            viewportPosition = new Vector3();
        }
        
    }

}