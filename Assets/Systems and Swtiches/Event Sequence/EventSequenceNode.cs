using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    [System.Serializable]
    public class EventSequenceNode{
        public string id;
        [System.NonSerialized]
        public List<EventSequenceNode> parentNodes = new List<EventSequenceNode>();
        [System.NonSerialized]
        public List<EventSequenceNode> childrenNodes = new List<EventSequenceNode>();
        public ConditionalEventContainer sequencedEvent;
        public bool completed = false;
        public bool failedConditions = false;
        public bool allowFailiure = true;
        public List<string> parentIDs = new List<string>();
        public List<string> childIDs = new List<string>();

        public EventSequenceNode(){
            this.sequencedEvent = new ConditionalEventContainer();
        }

        public static EventSequenceNode Clone(EventSequenceNode original){
            ProvenceEventArgs eventClone = Object.Instantiate(original.sequencedEvent.provenceEvent);
            EventSequenceNode clone = (EventSequenceNode) original.MemberwiseClone();
            return clone;
        }

        public static List<EventSequenceNode> ReconstructNodeList(List<EventSequenceNode> list){
            foreach(EventSequenceNode node in list){
                for(int i = 0; i < node.parentNodes.Count; i++){
                    string id = node.parentNodes[i].id;
                    node.parentNodes[i] = list.Find(n => n.id.Equals(id));
                }

                for(int i = 0; i < node.childrenNodes.Count; i++){
                    string id = node.childrenNodes[i].id;
                    node.childrenNodes[i] = list.Find(n => n.id.Equals(id));
                }
            }
            return list;
        }

        public static List<EventSequenceNode> CloneList(List<EventSequenceNode> originalList){
            List<EventSequenceNode> cloneList = new List<EventSequenceNode>();
            for(int i = 0; i < originalList.Count; i++){
                cloneList.Add(EventSequenceNode.Clone(originalList[i]));
            }
            cloneList = EventSequenceNode.ReconstructNodeList(cloneList);
            return cloneList;
        }

    }
}