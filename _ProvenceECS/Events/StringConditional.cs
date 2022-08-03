using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class StringConditional{
        public string conditionName;
        public bool conditionState = true;

        public StringConditional(){
            conditionName = "";
        }

        public StringConditional (string conditionName){
            this.conditionName = conditionName;
        }
    }

}