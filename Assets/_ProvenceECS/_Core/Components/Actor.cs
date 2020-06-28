using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class Actor : ProvenceComponent{
        public float width;
        public float height;

        public Actor(){
            this.width = 1;
            this.height = 1;
        }

        public Actor(float width, float height){
            this.width = width;
            this.height = height;
        }
    }
}