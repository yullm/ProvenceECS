using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class ConditionalEventContainer{

        public ProvenceEventArgs provenceEvent;
        public List<StringConditional> switchList = new List<StringConditional>();
        public List<StringConditional> componentList = new List<StringConditional>();

        public ConditionalEventContainer(){
            this.provenceEvent = null;
        }

        public ConditionalEventContainer(ProvenceEventArgs provenceEvent){
            this.provenceEvent = provenceEvent;
        }

    }
}