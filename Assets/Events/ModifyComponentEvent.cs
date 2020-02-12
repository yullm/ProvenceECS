using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ModifyComponentEvent : ProvenceEventArgs{
        
        public enum ModifyType {ADD, EDIT, REMOVE, REMOVE_ALL};
        public GameObject reflectionPool;
        public Component reflection;
        public Component chosenComponent;
        public bool active = true;
        public bool[] editStates;
        public ModifyType modifyType = ModifyType.EDIT;
        public Entity entityToModify;

        //Edit variables
        public bool editAddIfMissing = false;

        public void OnDestroy()
        {
            if(reflection != null) DestroyImmediate(reflection);
        }

    }
}