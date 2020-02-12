using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class EventSequenceSystem : ProvenceSystem{

        private Dictionary<string,List<EventSequenceNode>> runningSequences = new Dictionary<string, List<EventSequenceNode>>();
        
        public override void Initialize(WorldRegistrationComplete args){
            world.eventManager.AddListener<StartEventSequence>(StartEventSequence);
            world.eventManager.AddListener<SequenceEventCompleted>(EventCompleted);
            //world.eventManager.AddListener<CacheIntegrityChange>(IntegrityCheck);
        }

        private void StartEventSequence(StartEventSequence args){
            if(args.sequenceNodes == null || args.sequenceNodes.Count == 0){
                Debug.Log("Empty node sequence");
                return;
            }
            args.sequenceNodes = EventSequenceNode.CloneList(args.sequenceNodes);
            for(int i = 0; i < args.sequenceNodes.Count; i++){
                args.sequenceNodes[i].sequencedEvent.provenceEvent.conditionalEntity = args.conditionalEntity;
            }
            string sequenceID = Guid.NewGuid().ToString();
            Debug.Log(sequenceID);
            runningSequences[sequenceID] = args.sequenceNodes;
            List<EventSequenceNode> startingNodes = FindStartingNodes(args.sequenceNodes);
            foreach(EventSequenceNode node in startingNodes) ProcessEventNode(sequenceID,node);
        }

        private void ProcessEventNode(string sequenceID, EventSequenceNode currentNode){
            //Debug.Log("Processing event: " + currentNode.sequencedEvent.provenceEvent.GetType().Name);
            if(currentNode.parentNodes.Count > 0){
                int completeCount = 0;
                for(int i = 0; i < currentNode.parentNodes.Count; i++){
                    if(currentNode.parentNodes[i].completed) completeCount++;
                    else{
                        if(currentNode.parentNodes[i].failedConditions){
                            if(currentNode.parentNodes[i].allowFailiure) completeCount++;
                            else CheckIfCompleted(sequenceID);
                        }
                    }
                }
                if(completeCount != currentNode.parentNodes.Count) return;
            }
            bool switchesValid = world.switchManager.ValidateSwitchConditons(currentNode.sequencedEvent.switchList);
            bool componentsValid = world.componentManager.ValidateConditionalComponentList(currentNode.sequencedEvent.provenceEvent.conditionalEntity, currentNode.sequencedEvent.componentList);
            if(switchesValid && componentsValid){
                currentNode.sequencedEvent.provenceEvent.OnCompleteCallback = SequenceEventCompleted.CreateInstance(sequenceID, currentNode);
                new EventHandle(world,currentNode.sequencedEvent.provenceEvent).Raise();
            }else{
                Debug.Log("failed conditions");
                currentNode.failedConditions = true;
                if(currentNode.allowFailiure) EventCompleted(SequenceEventCompleted.CreateInstance(sequenceID,currentNode));
                else CheckIfCompleted(sequenceID);
            }
        }

        private void EventCompleted(SequenceEventCompleted args){
            args.completedEventNode.completed = true;
            if(args.completedEventNode.childrenNodes.Count == 0){
                CheckIfCompleted(args.sequenceID);
                return;
            }
            foreach(EventSequenceNode child in args.completedEventNode.childrenNodes){
                ProcessEventNode(args.sequenceID,child);
            }
        }

        private void CheckIfCompleted(string sequenceID){
            if(!runningSequences.ContainsKey(sequenceID)) return;
            List<EventSequenceNode> endingNodes = FindEndingNodes(runningSequences[sequenceID]);
            int completeCount = 0;
            for(int i = 0; i < endingNodes.Count; i++){
                if(endingNodes[i].completed) completeCount++;
            }
            if(completeCount == endingNodes.Count){
                runningSequences.Remove(sequenceID);
                return;
            }
            List<EventSequenceNode> startingNodes = FindStartingNodes(runningSequences[sequenceID]);
            for(int i = 0; i < startingNodes.Count; i++){
                if(CheckForPath(startingNodes[i])) return;
            }
            runningSequences.Remove(sequenceID);
        }

        private bool CheckForPath(EventSequenceNode node){
            if(node.failedConditions && !node.allowFailiure) return false;
            if(node.childrenNodes.Count == 0) return !node.completed;
            for(int i = 0; i < node.childrenNodes.Count; i++){
                if(CheckForPath(node.childrenNodes[i])) return true;
            }
            return false;
        }

        private List<EventSequenceNode> FindStartingNodes(List<EventSequenceNode> nodeList){
            List<EventSequenceNode> startingNodes = new List<EventSequenceNode>();
            for(int i = 0; i < nodeList.Count; i++){
                if(nodeList[i].parentNodes.Count == 0) startingNodes.Add(nodeList[i]);
            }
            return startingNodes;
        }

        private List<EventSequenceNode> FindEndingNodes(List<EventSequenceNode> nodeList){
            List<EventSequenceNode> endingNodes = new List<EventSequenceNode>();
            for(int i = 0; i < nodeList.Count; i++){
                if(nodeList[i].childrenNodes.Count == 0) endingNodes.Add(nodeList[i]);
            }
            return endingNodes;
        }

        public override void GatherCache(){
            throw new NotImplementedException();
        }

        public override void IntegrityCheck(CacheIntegrityChange args){
            throw new NotImplementedException();
        }
    }

}