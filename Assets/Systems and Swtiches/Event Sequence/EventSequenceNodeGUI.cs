using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProvenceECS{
    [System.Serializable]
    public class EventSequenceNodeGUI{
        public string id;
        public EventSequenceNode node;

        public Rect rect;
        public string title;
        public int width = 200;
        public int height = 50;

        public GUIStyle style;

        public bool isDragged;
        public bool selected;

        public EventSequenceConnectionPoint inPoint;
        public EventSequenceConnectionPoint outPoint;

        public EventSequenceNodeGUI(Vector2 position, GUIStyle style, GUIStyle pointStyle){
            //this.title = "Empty";
            this.node = new EventSequenceNode();
            this.rect = new Rect(position.x, position.y, width, height);
            this.style = style;
            this.inPoint = new EventSequenceConnectionPoint(this, EventSequenceConnectionPointType.IN, pointStyle);
            this.outPoint = new EventSequenceConnectionPoint(this, EventSequenceConnectionPointType.OUT, pointStyle);
            this.id = id ?? Guid.NewGuid().ToString();
            this.node.id = id;
        }

        public void SetConnectionPoints(){
            inPoint.node = this;
            outPoint.node = this;
        }

        public void AddParent(EventSequenceConnectionPoint point){
            node.parentNodes.Add(point.node.node);
        }

        public void AddChild(EventSequenceConnectionPoint point){
            node.childrenNodes.Add(point.node.node);
        }

        public void RemoveParent(EventSequenceConnectionPoint point){
            node.parentNodes.Remove(point.node.node);
            }

        public void RemoveChild(EventSequenceConnectionPoint point){
            node.childrenNodes.Remove(point.node.node);
        }

        public void ClearNode(){
            
            for(int i = 0; i < node.parentNodes.Count; i++)
                node.parentNodes[i].childrenNodes.Remove(node);

            for(int i = 0; i < node.childrenNodes.Count; i++)
                node.childrenNodes[i].parentNodes.Remove(node);

        }

    }

}