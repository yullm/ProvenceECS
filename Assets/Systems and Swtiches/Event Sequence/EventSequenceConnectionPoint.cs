using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public enum EventSequenceConnectionPointType {IN,OUT};

    [System.Serializable]
    public class EventSequenceConnectionPoint{

        public string id;
        public Rect rect;
        public EventSequenceConnectionPointType type;
        [System.NonSerialized]
        public EventSequenceNodeGUI node;
        public GUIStyle style;
 

        public EventSequenceConnectionPoint(EventSequenceNodeGUI node, EventSequenceConnectionPointType type, GUIStyle style){
            this.node = node;
            this.type = type;
            this.style = style;
            this.rect = new Rect(0,0,10f,20f);
            this.id = id ?? Guid.NewGuid().ToString();
        }


    }

}