using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    public class TaskWaitTest : ProvenceEventArgs{
        public string thing = "bikle";
    };

    // Systems must be in own file
    public class TestSystem : ProvenceSystem {
        
        public override void Initialize(WorldRegistrationComplete args){
            world.eventManager.AddListener<InteractPressed>(Test);
        }

        public async void Test(InteractPressed args){
            world.eventManager.RemoveListener<InteractPressed>(Test);
            SelectPressed t = await world.eventManager.WaitForEvent<SelectPressed>();
            Debug.Log(t);
            world.eventManager.AddListener<InteractPressed>(Test);
        }
    }

}