using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class SetSwitchEvent : ProvenceEventArgs{
        public string switchName;

        public SetSwitchEvent(string switchName){
            this.switchName = switchName;
        }
    }
}