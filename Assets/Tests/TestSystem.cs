using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ProvenceECS{
    // Systems must be in own file
    public class TestSystem : ProvenceSystem {
        
        public Vector3 testLocation;
        
        public override void Initialize(WorldRegistrationComplete args){
            //world.eventManager.AddListener<HoldRequestEvent>(Test);
        }

        public void Test(){
            Debug.Log("holding?");
        }

        public override void GatherCache(){}

        public override void IntegrityCheck(CacheIntegrityChange args){}
    }

}