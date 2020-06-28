using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ProvenceECS{

    public class TestEvent : ProvenceEventArgs{
        public int index;

        public TestEvent(){
            this.index = 0;
        }

        public TestEvent(int index){
            this.index = index;
        }
    }

    public class TestSystem : ProvenceSystem {
        /* // All layers except 8 and 9
        layerMask = ~((1 << 8) | (1 << 9)); */

        protected EntityHandle testHandle;
                
        public override void Initialize(WorldRegistrationComplete args){
            if(Application.isPlaying) world.eventManager.AddListener<AllSystemsReadyEvent>(Test);         
            world.eventManager.Raise<SystemReadyEvent>(new SystemReadyEvent(this));
        }

        protected void Test(AllSystemsReadyEvent args){
            testHandle = world.CreateEntity();
            GameObject go = testHandle.AddGameObject();
            NavMeshSurface surface = go.AddComponent<NavMeshSurface>();
            surface.layerMask = ~(1 << 8);
            surface.BuildNavMesh();
        }

        public override void GatherCache(){}

        public override void IntegrityCheck(CacheIntegrityChange args){}

    }

}