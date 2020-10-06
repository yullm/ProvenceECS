using UnityEngine;

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
 
        public Vector3 size = new Vector3(5,5,5);
        public Vector3 offset;

        protected Texture2D[] hitMaps;

        public Vector3 pos;
        public float radialDistance = 3;

                
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<WakeSystemEvent>(Awaken);    
            world.eventManager.AddListener<WorldUpdateEvent>(Tick);
            world.eventManager.RegisterReturnMethod<TestEvent,bool>(ReturnTest);     
        }

        protected bool ReturnTest(TestEvent args){
            return false;
        }

        public override void Awaken(WakeSystemEvent args){
            Debug.Log(world.eventManager.RaiseReturn<TestEvent,bool>(new TestEvent()));
        }


        protected void Tick(WorldUpdateEvent args){

        }

        protected override void GatherCache(){}

    }

}