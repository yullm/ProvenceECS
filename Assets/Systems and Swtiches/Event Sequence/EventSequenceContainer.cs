using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class EventSequenceContainer : ISerializationCallbackReceiver{

        public List<EventSequenceNodeGUI> nodes = new List<EventSequenceNodeGUI>();
        public List<EventSequenceConnection> connections = new List<EventSequenceConnection>();
        
        public void OnBeforeSerialize(){
            foreach(EventSequenceNodeGUI guiNode in nodes){
                guiNode.node.parentIDs.Clear();
                guiNode.node.childIDs.Clear();
                for(int i = 0; i < guiNode.node.parentNodes.Count; i++)
                    guiNode.node.parentIDs.Add(guiNode.node.parentNodes[i].id);
                for(int i = 0; i < guiNode.node.childrenNodes.Count; i++)
                    guiNode.node.childIDs.Add(guiNode.node.childrenNodes[i].id);
            }
        }

        public void OnAfterDeserialize(){
            connections.Clear();
            foreach(EventSequenceNodeGUI guiNode in nodes){
                guiNode.inPoint.node = guiNode;
                guiNode.outPoint.node = guiNode;
            }
            foreach(EventSequenceNodeGUI guiNode in nodes){
                guiNode.node.parentNodes = new List<EventSequenceNode>();
                for(int i = 0; i < guiNode.node.parentIDs.Count; i++){
                    EventSequenceNodeGUI parentNode = nodes.Find(n => n.id.Equals(guiNode.node.parentIDs[i]));
                    guiNode.node.parentNodes.Add(parentNode.node);
                    connections.Add(new EventSequenceConnection( guiNode.inPoint, parentNode.outPoint) );
                }
                guiNode.node.childrenNodes = new List<EventSequenceNode>();
                for(int i = 0; i < guiNode.node.childIDs.Count; i++) 
                    guiNode.node.childrenNodes.Add(nodes.Find(n => n.id.Equals(guiNode.node.childIDs[i])).node);
            }
            
        }

        
    }

}