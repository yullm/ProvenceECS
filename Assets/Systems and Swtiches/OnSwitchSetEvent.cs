using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProvenceECS{

    public class OnSwitchSetEvent : ProvenceEventArgs{
       public string switchName;

        public static OnSwitchSetEvent CreateInstance(){
            OnSwitchSetEvent instance = ScriptableObject.CreateInstance<OnSwitchSetEvent>();
            instance.switchName = "";
            return instance;
        }

        public new static OnSwitchSetEvent CreateInstance(string switchName){
            OnSwitchSetEvent instance = ScriptableObject.CreateInstance<OnSwitchSetEvent>();
            instance.switchName = switchName;
            return instance;
        }
    }
}
