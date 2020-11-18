using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                 
        protected override void RegisterEventListeners(){}

        protected override void GatherCache(){}

    }

}