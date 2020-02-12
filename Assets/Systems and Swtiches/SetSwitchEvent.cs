using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class SetSwitchEvent : ProvenceEventArgs{
       public string switchName;

        public static SetSwitchEvent CreateInstance(){
            SetSwitchEvent instance = ScriptableObject.CreateInstance<SetSwitchEvent>();
            instance.switchName = "";
            return instance;
        }

        public new static SetSwitchEvent CreateInstance(string switchName){
            SetSwitchEvent instance = ScriptableObject.CreateInstance<SetSwitchEvent>();
            instance.switchName = switchName;
            return instance;
        }
    }
}