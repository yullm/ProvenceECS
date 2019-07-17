using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class TestComponentA : ProvenceComponent{
        public int x = 0;
        public float y = 0;
    }

    public class TestComponentB : ProvenceComponent{
        public Vector3 up = new Vector3(0,1,0);
        public Vector3 down = new Vector3(0,-1,0);
    }

}