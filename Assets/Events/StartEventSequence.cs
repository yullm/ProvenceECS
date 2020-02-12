using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class StartEventSequence : ProvenceEventArgs{
        
        public List<EventSequenceNode> sequenceNodes;

        public static StartEventSequence CreateInstance(List<EventSequenceNode> sequenceNodes, Entity conditionalEntity){
            StartEventSequence instance = ScriptableObject.CreateInstance<StartEventSequence>();
            instance.sequenceNodes = sequenceNodes;
            instance.conditionalEntity = conditionalEntity;
            return instance;
        }
        
        public static StartEventSequence CreateInstance(List<EventSequenceNodeGUI> guiNodes, Entity conditionalEntity){
            List<EventSequenceNode> nodes = new List<EventSequenceNode>();
            for(int i = 0; i < guiNodes.Count; i++){
                nodes.Add(guiNodes[i].node);
            }
            StartEventSequence instance = ScriptableObject.CreateInstance<StartEventSequence>();
            instance.sequenceNodes = nodes;
            instance.conditionalEntity = conditionalEntity;
            return instance;
        }

    }

}