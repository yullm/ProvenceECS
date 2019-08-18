using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    // Systems must be in own file
    public class TestSystem : ProvenceSystem {
        public float timer = 0f;
        public float target = 1f;

        public override void RegisterForEvents(WorldRegistrationComplete args){
            world.eventManager.AddListener<WorldUpdateEvent>(Test);
        }

        public void Test(WorldUpdateEvent args){
            timer += Time.deltaTime;
            if(timer >= target){
                Debug.Log("Boop");
                EntityHandle entity = world.CreateEntity();
                ComponentHandle<TestComponents> testComponent = entity.AddComponent<TestComponents>(4);
                Debug.Log(testComponent.component.x);
                timer = 0;
            }
        }
    }

}