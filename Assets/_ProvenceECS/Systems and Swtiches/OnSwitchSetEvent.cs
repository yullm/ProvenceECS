using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProvenceECS{

    public class OnSwitchSetEvent : ProvenceEventArgs{
       public string switchName;

        public OnSwitchSetEvent(string switchName){
            this.switchName = switchName;
        }
    }
}
