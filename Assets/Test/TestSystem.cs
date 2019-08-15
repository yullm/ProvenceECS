using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    // Systems must be in own file
    public class TestSystem : ProvenceSystem {
        public int testVariable;

        public override void Init(){
            world.eventManager.AddListener<WorldUpdateEvent>(Test);
        }

        public void Test(WorldUpdateEvent args){
            Debug.Log(world.name);
        }
    }

}