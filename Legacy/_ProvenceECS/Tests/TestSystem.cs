using UnityEngine;
using ProvenceECS.Network;
using System.Collections.Generic;
using Sjena.Movement;
using Ransacked.AI;
using Sjena.Mainframe;
using ProvenceECS.Mainframe;
using Sjena.Selection;
using Sjena.GameManagement;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Ransacked;

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

        /* public override async void Awaken(WakeSystemEvent args){
            await Task.Delay(2000);
            foreach(Entity entity in world.componentManager.GetAllComponentsAsDictionary<Unit>().Keys){
                new ExecuteAttack(new Sjena.Stats.Attack(entity,entity,Sjena.Stats.ActionSource.ATTACK,40)).Raise(world);
            }
        } */

        protected void TestEvent(TestEvent<ProvenceComponent> args){
            Debug.Log("We're here. We did it, I think.");
        }

        protected void TestEvent(TestEvent<ComponentA> args){
            Debug.Log("We really did it i think.");
        }

    }

    public class TestSystemB : ProvenceSystem{

        public Entity tempActor;
        public Entity tempTile;
        public ProvenceAsset<Texture2D> asset;

        public TestSystemB(){
            this.tempActor = null;
            this.tempTile = null;
            this.asset = new();
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
                new SetControlGroup(0,new(){tempActor}).Raise(world);

                if(tempTile != null && false){
                    float distance = 6;

                    ComponentCache<UnityGameObject> goCache = new();
                    goCache.GatherCache(world);
                    
                    ComponentCache<Tile> tileCache = new();
                    tileCache.GatherCache(world);

                    Vector3 startingPosition = goCache[tempTile].component.gameObject.transform.position.Snap();

                    foreach(ComponentHandle<Tile> tileHandle in tileCache.Values){
                        if(tileHandle.entity == tempTile) continue;
                        Vector3 position = goCache[tileHandle.entity].component.gameObject.transform.position.Snap();
                        if(SjenaMainframe.RoundingDistance(startingPosition,position,distance)){
                            Model model = world.GetComponent<Model>(tileHandle.entity)?.component;
                            if(model != null){
                                HashSet<Material> highlightMats = new ();
                                highlightMats = model.renderers.Select(r => r.material).ToSet();
                                foreach(Material highlight in highlightMats){
                                    highlight.SetColor("_Highlight", ColourKeys.ransackedRed);
                                }
                            }
                        }
                    }
                }
            } */
            
        }

        protected void Tick(WorldUpdateEvent args){
            int count = 5;
            List<string> columnNames = new();
            List<string> reader = new();
            for(int i = 0; i < count; i++)
            Debug.Log($"{columnNames[i]}: {reader[i]}");
        }  
        
    }

}