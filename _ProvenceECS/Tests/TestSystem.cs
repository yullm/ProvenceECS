using UnityEngine;
using ProvenceECS.Network;
using System.Collections.Generic;
using Sjena.Movement;
using Ransacked.AI;
using Sjena.Mainframe;
using ProvenceECS.Mainframe;

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
        //public ComponentB b;

        public ComponentA(){
            this.number = 5;
            //this.b = null;
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

        public override void Awaken(WakeSystemEvent args){
            /* Dictionary<Entity,ProvenceComponent> cache = world.componentManager.GetEntry<ComponentA>();
            Debug.Log(cache.Count);
            EntityHandle handle = world.CreateEntity();
            handle.AddComponent<ComponentA>();
            Debug.Log(cache.Count); */

        }

        protected void TestEvent(TestEvent<ProvenceComponent> args){
            Debug.Log("We're here. We did it, I think.");
        }

        protected void TestEvent(TestEvent<ComponentA> args){
            Debug.Log("We really did it i think.");
        }

    }

    public class TestSystemB : ProvenceSystem{

        public Entity tempActor;
        protected GameObject tempActorGO;

        public TestSystemB(){
            this.tempActor = null;
        }

        protected override void RegisterEventListeners(){
            //world.eventManager.RemoveListener<WakeSystemEvent>(Awaken);            
            if(Application.isPlaying){
                //world.eventManager.AddListener<WakeSystemEvent>(Awaken,1);
                world.eventManager.AddListener<WorldUpdateEvent>(Tick);
            }
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<WorldUpdateEvent>(Tick);
        }

        public override void Awaken(WakeSystemEvent args){
            /* if(Application.isPlaying){
                FactionMember member = world.AddComponent(tempActor, new FactionMember(FactionDataKeys.Primary)).component;

                EntityHandle newEntity = world.CreateEntity();
                FactionMember newMember = newEntity.AddComponent(new FactionMember(FactionDataKeys.Primary)).component;

                Debug.Log(newMember.factionData.alignment);
                member.factionData.alignment = FactionAlignment.EVIL;
                Debug.Log(newMember.factionData.alignment);
            } */
        }

        protected void Tick(WorldUpdateEvent args){
            //HeightTick(args);
            if(Input.GetMouseButtonDown(0)){
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Tile"))){
                    Vector3 point = hit.collider.gameObject.transform.parent.transform.position.Snap();
                    Entity tileEntity = world.eventManager.RaiseReturn<TileAtPointCheck,Entity>(new (point));
                    if(tileEntity != null)TestFindPath(tileEntity);
                }
            }
        }  

        protected void TestFindPath(Entity goal){   
            TilePath path = world.eventManager.RaiseReturn<FindPathRequest,TilePath>(new FindPathRequest(tempActor,goal));
            if(path != null){
                new RegisterQueueAction(tempActor, new PathQueueAction(path)).Raise(world);
            }
        }
        
    }

}