using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ProvenceECS{

    public class EventSequenceGraph : EditorWindow{

        private EventSequenceContainer sequenceContainer;
        private string descriptor;
        
        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;
        private GUIStyle pointStyle;

        private EventSequenceConnectionPoint selectedInPoint;
        private EventSequenceConnectionPoint selectedOutPoint;

        private Vector2 drag;
        private Vector2 offset;

        public static void Show(EventSequenceContainer sequenceContainer, string descriptor){
            EventSequenceGraph window = (EventSequenceGraph)GetWindow<EventSequenceGraph>();
            window.sequenceContainer = sequenceContainer;
            window.descriptor = descriptor;
        }

        private void OnEnable(){

            sequenceContainer = null;
            selectedOutPoint = null;
            selectedInPoint = null;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12,12,12,12);

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            pointStyle = new GUIStyle();
            pointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            pointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            pointStyle.border = new RectOffset(4, 4, 12, 12);

            AssemblyReloadEvents.beforeAssemblyReload += () => Close();

        }

        private void OnDisable(){
            AssemblyReloadEvents.beforeAssemblyReload -= () => Close();
        }

        private void OnGUI(){
            if(Application.isPlaying) Close();
            DrawGrid(20, new Color(0.5f,0.5f,0.5f,0.2f));
            DrawGrid(80, new Color(0.5f,0.5f,0.5f,0.4f));

            if(sequenceContainer == null) return;

            DrawGraph(Event.current);
            ProcessGraphEvents(Event.current);
            
            if(GUI.changed) Repaint();
        }
        
        private void DrawGraph(Event e){
            DrawNodes();
            DrawConnections();
            DrawConnectionLine(e);
        }

        private void DrawNodes(){
            if(sequenceContainer.nodes != null){
                for(int i = 0; i < sequenceContainer.nodes.Count; i++){
                    string label = "";
                    Rect labelRect = sequenceContainer.nodes[i].rect;
                    labelRect.position += new Vector2(10,-2);
                    labelRect.width -= 30;
                    if(sequenceContainer.nodes[i].node.sequencedEvent.provenceEvent != null) label = sequenceContainer.nodes[i].node.sequencedEvent.provenceEvent.GetType().Name;
                    GUI.Box(sequenceContainer.nodes[i].rect,sequenceContainer.nodes[i].title,sequenceContainer.nodes[i].style);
                    GUI.Label(labelRect,label);
                    DrawConnectionPoint(sequenceContainer.nodes[i],sequenceContainer.nodes[i].inPoint);
                    DrawConnectionPoint(sequenceContainer.nodes[i],sequenceContainer.nodes[i].outPoint);
                }
            }
        }

        private void DrawConnectionPoint(EventSequenceNodeGUI node, EventSequenceConnectionPoint point){
            point.rect.y = node.rect.y + (node.rect.height * 0.5f) - point.rect.height * 0.5f;
            switch(point.type){
                case EventSequenceConnectionPointType.IN:
                    point.rect.x = node.rect.x - point.rect.width + 8f;
                    if(GUI.Button(point.rect,"",point.style))
                        OnClickInPoint(point);
                    break;
                case EventSequenceConnectionPointType.OUT:
                    point.rect.x = node.rect.x + node.rect.width - 8f;
                    if(GUI.Button(point.rect,"",point.style))
                        OnClickOutPoint(point);
                    break;
            }
            
        }

        private void DrawConnections(){
            for(int i = 0; i < sequenceContainer.connections.Count; i++){
                EventSequenceConnection connection = sequenceContainer.connections[i];
                Vector3 inTangent = connection.inPoint.rect.center + Vector2.left * 50f;
                Vector3 outTangent = connection.outPoint.rect.center - Vector2.left * 50f;
                Handles.DrawBezier(connection.inPoint.rect.center, connection.outPoint.rect.center, inTangent, outTangent, Color.white, null, 2f);

                if(Handles.Button((connection.inPoint.rect.center + connection.outPoint.rect.center) * 0.5f,Quaternion.identity, 4f, 8f, Handles.RectangleHandleCap)){
                    OnClickRemoveConnection(connection);
                }
            }
            
        }

        private void DrawConnectionLine(Event e){
            if(selectedInPoint != null && selectedOutPoint == null){
                Vector3 inTangent = selectedInPoint.rect.center + Vector2.left * 50f;
                Vector3 mouseTangent = e.mousePosition - Vector2.left * 50f;
                Handles.DrawBezier(selectedInPoint.rect.center,e.mousePosition,inTangent,mouseTangent,Color.white,null,2f);
                GUI.changed = true;
            }

            if(selectedInPoint == null && selectedOutPoint != null){
                Vector3 outTangent = selectedOutPoint.rect.center - Vector2.left * 50f;
                Vector3 mouseTangent = e.mousePosition + Vector2.left * 50f;
                Handles.DrawBezier(selectedOutPoint.rect.center,e.mousePosition,outTangent,mouseTangent,Color.white,null,2f);
                GUI.changed = true;
            }

        }

        private void DrawGrid(float gridSpacing, Color gridColour){
            
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
            
            Handles.BeginGUI();

            Handles.color = gridColour;
            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
            for(int i = 0; i <= widthDivs; i++){
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height + 30, 0f) + newOffset);
            }
            for(int i = 0; i <= widthDivs; i++){
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i,  0) + newOffset, new Vector3(position.width + 30, gridSpacing * i,  0f) + newOffset);
            }
            Handles.color = Color.white;            

            Handles.EndGUI();
        }

        private void OnDrag(Vector2 delta){
            drag = delta;
            for(int i = 0; i < sequenceContainer.nodes.Count; i++){
                DragNode(sequenceContainer.nodes[i],delta);
            }
            GUI.changed = true;
        }

        private void DragNode(EventSequenceNodeGUI node, Vector2 delta){
            node.rect.position += delta;
        }

        private void ProcessGraphEvents(Event e){
            ProcessNodeEvents(e);
            ProcessGUIEvents(e);
        }

        private void ProcessGUIEvents(Event e){
            drag = Vector2.zero;
            switch(e.type){
                case EventType.MouseDown:
                    if(e.button == 0)
                        ClearConnectionSelection();
                    if(e.button == 1){
                        ProcessEditorContextMenu(e.mousePosition);            
                    }
                    break;
                case EventType.MouseDrag:
                    OnDrag(e.delta);
                    break;
            }
        }
        
        private void ProcessNodeEvents(Event e){
            foreach(EventSequenceNodeGUI node in sequenceContainer.nodes){
                switch(e.type){
                    case EventType.MouseDown:
                        if(e.button == 0){
                            if(node.rect.Contains(e.mousePosition)){
                                node.isDragged = true;
                                node.selected = true;
                                node.style = selectedNodeStyle;
                                if(e.clickCount == 2){
                                    EventSequenceNodeEditor.Show(node.node,descriptor);
                                }
                            }else{
                                node.selected = false;
                                node.style = nodeStyle;
                            }
                            GUI.changed = true;
                        }
                        if(e.button == 1 && node.selected && node.rect.Contains(e.mousePosition)){
                            ProcessNodeContextMenu(node);
                            e.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        node.isDragged = false;
                        break;
                    case EventType.MouseDrag:
                        if(e.button == 0 && node.isDragged){
                            DragNode(node,e.delta);
                            e.Use();
                            GUI.changed =  true;
                        }
                        break;
                }

            }
        }
        
        private void ProcessNodeContextMenu(EventSequenceNodeGUI node){
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove Node"),false,() => OnClickRemoveNode(node));
            genericMenu.ShowAsContext();
        }
        
        private void ProcessEditorContextMenu(Vector2 mousePosition){
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Node"),false,() => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 mousePosition){
            sequenceContainer.nodes.Add(new EventSequenceNodeGUI(mousePosition, nodeStyle, pointStyle));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void OnClickRemoveNode(EventSequenceNodeGUI node){
            List<EventSequenceConnection> connectionsToRemove = new List<EventSequenceConnection>();
            for(int i = 0; i < sequenceContainer.connections.Count; i++){
                if(sequenceContainer.connections[i].inPoint == node.inPoint || sequenceContainer.connections[i].outPoint == node.outPoint)
                    connectionsToRemove.Add(sequenceContainer.connections[i]);
            }
            for(int i = 0; i < connectionsToRemove.Count; i++){
                sequenceContainer.connections.Remove(connectionsToRemove[i]);
            }
            node.ClearNode();
            sequenceContainer.nodes.Remove(node);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void OnClickInPoint(EventSequenceConnectionPoint inPoint){
            selectedInPoint = inPoint;
            if(selectedOutPoint != null){
                if(selectedOutPoint.node != selectedInPoint.node){
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else ClearConnectionSelection();
            }
        }

        private void OnClickOutPoint(EventSequenceConnectionPoint outPoint){
            selectedOutPoint = outPoint;
            if(selectedInPoint != null){
                if(selectedOutPoint.node != selectedInPoint.node){
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else ClearConnectionSelection();
            }
        }

        private void OnClickRemoveConnection(EventSequenceConnection connection){
            connection.inPoint.node.RemoveParent(connection.outPoint);
            connection.outPoint.node.RemoveChild(connection.inPoint);
            sequenceContainer.connections.Remove(connection);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void CreateConnection(){
            selectedOutPoint.node.AddChild(selectedInPoint);
            selectedInPoint.node.AddParent(selectedOutPoint);
            foreach(EventSequenceConnection connection in sequenceContainer.connections){
                if(connection.inPoint == selectedInPoint && connection.outPoint == selectedOutPoint) return;
            }
            sequenceContainer.connections.Add(new EventSequenceConnection(selectedInPoint, selectedOutPoint));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void ClearConnectionSelection(){
            selectedInPoint = null;
            selectedOutPoint = null;
        }
        
    }

}