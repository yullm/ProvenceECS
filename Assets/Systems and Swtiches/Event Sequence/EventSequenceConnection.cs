using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS{
    [System.Serializable]
    public class EventSequenceConnection{
        public string id;
        public EventSequenceConnectionPoint inPoint;
        public EventSequenceConnectionPoint outPoint;
        public Action<EventSequenceConnection> OnClickRemoveConnecton;

        public EventSequenceConnection(EventSequenceConnectionPoint inPoint, EventSequenceConnectionPoint outPoint){
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.id = id ?? Guid.NewGuid().ToString();
        }

    }

}