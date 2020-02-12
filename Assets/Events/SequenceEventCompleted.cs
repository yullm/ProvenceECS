using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class SequenceEventCompleted : ProvenceEventArgs{
        
        public string sequenceID;
        public EventSequenceNode completedEventNode;

        public static SequenceEventCompleted CreateInstance(string sequenceID, EventSequenceNode completedEventNode){
            SequenceEventCompleted instance = ScriptableObject.CreateInstance<SequenceEventCompleted>();
            instance.sequenceID = sequenceID;
            instance.completedEventNode = completedEventNode;
            return instance;
        }
        
    }

}