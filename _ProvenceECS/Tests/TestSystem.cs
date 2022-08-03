using UnityEngine;
using ProvenceECS.Network;

namespace ProvenceECS{
    [ProvencePacket(666)]
    public class TestEvent<ProvenceComponent> : ProvenceEventArgs{
        public int index;

        public TestEvent(){
            this.index = 0;
        }

        public TestEvent(int index){
            this.index = index;
        }
    }

    public class ComponentA : ProvenceComponent{

        public int number;
        public ComponentB b;

        public ComponentA(){
            this.number = 5;
            this.b = null;
        }

    }

    public class ComponentB : ProvenceComponent{
        public string text;
        public ComponentA a;

        public ComponentB(ComponentA a){
            this.text = "Text";
            this.a = a;
        }
    }

    public class TestSystem : ProvenceSystem {
        
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<TestEvent<ProvenceComponent>>(TestEvent);
            world.eventManager.AddListener<TestEvent<ComponentA>>(TestEvent);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<TestEvent<ProvenceComponent>>(TestEvent);
            world.eventManager.RemoveListener<TestEvent<ComponentA>>(TestEvent);
        }

        protected void TestEvent(TestEvent<ProvenceComponent> args){
            Debug.Log("We're here. We did it, I think.");
        }

        protected void TestEvent(TestEvent<ComponentA> args){
            Debug.Log("We really did it i think.");
        }

    }

    public class TestSystemB : ProvenceSystem{
        
        public override void Awaken(WakeSystemEvent args){}

        protected override void RegisterEventListeners(){
            
        }

        protected override void DeregisterEventListeners(){

        }
        
    }

}