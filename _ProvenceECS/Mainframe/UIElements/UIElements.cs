using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor.SceneManagement;
using System;
using System.Threading.Tasks;

namespace ProvenceECS.Mainframe{

    public abstract class MainframeUIArgs {}

    //Events

    public class MouseClickEvent : MainframeUIArgs{
        public VisualElement element;
        public int button;
        public Vector2 mousePosition;
        public MouseUpEvent unityEvent;

        public MouseClickEvent(){
            this.element = null;
            this.button = -1;
            this.mousePosition = new Vector2(0,0);
            this.unityEvent = null;
        }
               
        public MouseClickEvent(VisualElement element, int button, Vector2 mousePosition, MouseUpEvent unityEvent){
            this.element = element;
            this.button = button;
            this.mousePosition = mousePosition;
            this.unityEvent = unityEvent;
        }

        public MouseClickEvent(VisualElement element, MouseUpEvent unityEvent){
            this.element = element;
            this.button = unityEvent.button;
            this.mousePosition = unityEvent.mousePosition;
            this.unityEvent = unityEvent;
        }
    }
    
    public class KeyPressEvent : MainframeUIArgs{

        public VisualElement element;
        public KeyCode keyCode;

        public KeyPressEvent(VisualElement element, KeyCode keyCode){
            this.element = element;
            this.keyCode = keyCode;
        }

    }

    public class ListItemInputChange : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputChange(VisualElement input){
            this.input = input;
        }
    }

    public class ListItemInputCommit : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputCommit(VisualElement input){
            this.input = input;
        }
    }

    public class ListItemInputCancel : MainframeUIArgs{
        public VisualElement input;
        public ListItemInputCancel(VisualElement input){
            this.input = input;
        }
    }

    public class LoseFocus : MainframeUIArgs{
        public VisualElement input;
        public LoseFocus(VisualElement input){
            this.input = input;
        }
    }


    public class Wrapper : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<Wrapper> {}

        public Wrapper(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("wrapper");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class Div : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<Div> {}

        public Div(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class MenuBar : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<MenuBar> {}

        public MenuBar(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("menu-bar");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class TableWrapper : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<TableWrapper> {}

        public TableWrapper(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("table-wrapper");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class PageColumn : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<PageColumn> {}

        public PageColumn(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("page-column");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class ColumnScroller : ScrollView{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ColumnScroller> {}

        public ColumnScroller(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("column-scroller");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

        public ColumnScroller(bool alternate) : this(){
            if(alternate) this.AddToClassList("alternate");
        }

    }

    public class DropDownMenu : VisualElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<DropDownMenu> {}

        public DropDownMenu(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.AddToClassList("drop-down-menu");
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise(new MouseClickEvent(this,e));
            });
            this.RegisterCallback<MouseLeaveEvent>(e => {
                this.style.display = DisplayStyle.None;
            });
        }

        public void Show(VisualElement root, MouseClickEvent e = null, bool changePosition = false){

            root.Query<DropDownMenu>().ForEach(current =>{
                current.style.display = DisplayStyle.None;
            });                

            if(changePosition){
                this.style.top = StyleKeyword.Auto;
                this.style.bottom = StyleKeyword.Auto;
                this.style.left = StyleKeyword.Auto;
                this.style.right = StyleKeyword.Auto;

                bool top = (e.mousePosition.y < (root.contentRect.height / 2));
                bool left = (e.mousePosition.x < (root.contentRect.width / 2));
                
                if(left){
                    this.style.left = e.mousePosition.x - 20 > 0 ? e.mousePosition.x - 20 : 0;
                }else{
                    float rightX = root.contentRect.width - e.mousePosition.x;
                    this.style.right = rightX - 20 > 0 ? rightX - 20 : 0;
                }
                if(top){
                    this.style.top = e.mousePosition.y - 20;
                }else{
                    this.style.bottom = e.mousePosition.y * -1;
                }              
                
            }
            this.style.display = DisplayStyle.Flex;
        }

        public void Show(VisualElement root, Vector2 position){
            root.Query<DropDownMenu>().ForEach(current =>{
                current.style.display = DisplayStyle.None;
            });

            
            this.style.bottom = StyleKeyword.Auto;
            this.style.right = StyleKeyword.Auto;

            this.style.top = position.y;
            this.style.left = position.x;

            this.style.display = DisplayStyle.Flex;
        }

    }

    public class img : VisualElement{

        public Texture image;
        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<img, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "src" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return null;
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((img)ve).SetImage(Resources.Load<Texture>(textAttribute.GetValueFromBag(bag, cc)));
            }
        }

        public img(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

        public img(string resource):this(){
            SetImage(Resources.Load<Texture>(resource));
        }

        public img SetImage(Texture texture){
            this.image = texture;
            this.style.backgroundImage = (StyleBackground)image;
            return this;
        }

    }

    public class ProvenceText : TextElement{

        public EventManager<MainframeUIArgs> eventManager;

        public new class UxmlFactory : UxmlFactory<ProvenceText, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits{
            UxmlStringAttributeDescription textAttribute = new UxmlStringAttributeDescription { name = "text" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription{
                get{
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc){
                base.Init(ve, bag, cc);
                ((ProvenceText)ve).text = textAttribute.GetValueFromBag(bag, cc);
            }
        }

        public ProvenceText() : base(){
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

        public ProvenceText(string text) : base(){
            this.text = text;
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.RegisterCallback<MouseUpEvent>(e =>{
                this.eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }

    }

    public class Node<T> : VisualElement{

        public Vector2 lastRestingPosition;

        public EventManager<MainframeUIArgs> eventManager;
        public T key;
        public ProvenceText label;
        public Texture icon;
        public byte layer;

        public new class UxmlFactory : UxmlFactory<Node<T>> {}

        public Node(){
            this.lastRestingPosition = new Vector2(0,0);
            
            this.eventManager = new EventManager<MainframeUIArgs>();
            this.key = default;
            this.icon = null;
            this.layer = 0;
            InitializeElement();
            RegisterEventListeners();
        }

        public Node(T key, Texture icon = null){
            this.key = key;
            this.icon = icon;

            this.lastRestingPosition = new Vector2(0,0);
            this.layer = 0;
            
            this.eventManager = new EventManager<MainframeUIArgs>();
            InitializeElement();
            RegisterEventListeners();
        }

        public void InitializeElement(){                   
            this.AddToClassList("node");

            Div iconEl = new Div();
            iconEl.style.backgroundImage = (StyleBackground)icon;
            iconEl.AddToClassList("node-icon");

            this.label = new ProvenceText("Untitled");
            this.label.AddToClassList("node-label");
            this.label.displayTooltipWhenElided = false;

            this.Add(iconEl,label);
        }

        public virtual void RegisterEventListeners(){
            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise<MouseClickEvent>(new MouseClickEvent(this,e));
            });
        }
    }

    public class RedrawNodeViewer: MainframeUIArgs{}

    public class NodeViewerSaveData<T>{
        public Vector2 anchorPosition;
        public Dictionary<T,Vector2> nodePositions;
        public Dictionary<T,byte> layerCache;
        public float scale;

        public NodeViewerSaveData(){
            this.nodePositions = new Dictionary<T, Vector2>();
            this.layerCache = new();
            this.anchorPosition = new Vector2();
            this.scale = 1f;
        }

        public NodeViewerSaveData(Node<T>[] nodeCache):this(){
            this.nodePositions = new Dictionary<T, Vector2>();
            for(int i = 0; i < nodeCache.Length; i++){
                nodePositions[nodeCache[i].key] = nodeCache[i].PositionToVector2();
                layerCache[nodeCache[i].key] = nodeCache[i].layer;
            }
        }
    }

    public class NodeViewer<T> : VisualElement{

        public bool windowFocused;

        protected Vector2 lastMousePosition;        
        protected Dictionary<T,Node<T>> nodeCache;
        protected Vector2 lastRestingPosition;
        protected bool isDragging;
        protected bool hasDraggedAnchor;

        public EventManager<MainframeUIArgs> eventManager;

        protected Div anchor;
        protected ProvenceText coordDisplay;
        protected ProvenceText resetButton;

        protected byte currentLayer;
        protected Vector2 gridSize;

        protected Vector2 mouseToAnchorPosition;

        public new class UxmlFactory : UxmlFactory<NodeViewer<T>> {}

        public NodeViewer(){             
            this.windowFocused = false;           
            this.nodeCache = new Dictionary<T, Node<T>>();
            this.lastMousePosition = new Vector2(0,0);
            this.lastRestingPosition = new Vector2(0,0);
            this.isDragging = false;
            this.hasDraggedAnchor = false;
            this.eventManager = new EventManager<MainframeUIArgs>(); 

            this.currentLayer = 0;
            this.gridSize = new(140,50);
            
            this.mouseToAnchorPosition = new();

            InitializeElement();
            RegisterEventListeners();
        }

        protected virtual void InitializeElement(){            
            this.focusable = true;
            this.AddToClassList("node-viewer");
            
            anchor = new Div();
            anchor.AddToClassList("node-viewer-anchor");

            this.coordDisplay = new ProvenceText("[0,0]");
            this.coordDisplay.AddToClassList("node-viewer-coord");

            this.resetButton = new ProvenceText("[Reset]");
            this.resetButton.AddToClassList("node-viewer-reset");

            this.Add(anchor,coordDisplay,resetButton);
        }

        protected virtual void RegisterEventListeners(){
            this.RegisterCallback<MouseDownEvent>(e =>{
                if(e.button == 1) StartDragging(e);
            });

            this.RegisterCallback<MouseUpEvent>(e =>{
                eventManager.Raise(new MouseClickEvent(this,e));
                StopDragging(e.mousePosition);
            });

            this.RegisterCallback<MouseLeaveEvent>(e => {
                StopDragging(e.mousePosition);
                hasDraggedAnchor = true;
            });

            this.RegisterCallback<MouseMoveEvent>(e => {
                AnchorItemPositionFromMouse(e, out Vector2 pos);
                mouseToAnchorPosition = pos;
            });

            resetButton.RegisterCallback<MouseDownEvent>(e => {
                if(e.button == 0) ResetDrag();
            });
            
            this.RegisterCallback<WheelEvent>(e => {
                ZoomWindow(e);
            });
            this.RegisterCallback<KeyDownEvent>(e =>{
                if(e.target != this) return;
                if(e.keyCode == KeyCode.G){                    
                    e.StopPropagation();
                    if(e.ctrlKey) GridSelection();
                    FrameSelection();
                }
                if(e.keyCode == KeyCode.H){
                    e.StopPropagation();
                    if(e.ctrlKey) RowSelection();
                    else ColumnSelection();
                    FrameSelection();
                }
            });
        }

        public void LoadSaveData(NodeViewerSaveData<T> data){
            SetAnchorPosition(data.anchorPosition);
            anchor.style.left = data.anchorPosition.x;
            anchor.style.top = data.anchorPosition.y;
            coordDisplay.text = "[" + data.anchorPosition.x + "," + data.anchorPosition.y + "]";
            anchor.style.scale = new StyleScale(new Scale(new(data.scale,data.scale,data.scale)));

            foreach(KeyValuePair<T,Vector2> kvp in data.nodePositions){
                if(nodeCache.ContainsKey(kvp.Key)){
                    Node<T> node = nodeCache[kvp.Key];
                    node.style.left = kvp.Value.x;
                    node.style.top = kvp.Value.y;
                    node.lastRestingPosition = kvp.Value;
                    node.layer = data.layerCache.ContainsKey(kvp.Key) ? data.layerCache[kvp.Key] :  (byte)0;
                }
            }
        }

        protected void StartDragging(MouseDownEvent e){
            isDragging = true;
            lastMousePosition = e.mousePosition;
            this.RegisterCallback<MouseMoveEvent>(Drag);
        }

        protected void Drag(MouseMoveEvent e){
            Vector2 offset = e.mousePosition - lastMousePosition;
            if(offset.magnitude > 5) hasDraggedAnchor = true;
            SetAnchorOffset(offset);
        }

        protected void StopDragging(Vector2 mousePosition){
            if(isDragging){ 
                lastRestingPosition += mousePosition - lastMousePosition;
                if((mousePosition - lastMousePosition).magnitude > 5){
                    if(!Application.isPlaying)
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    
                }
            }
            this.UnregisterCallback<MouseMoveEvent>(Drag);
            isDragging = false;
            hasDraggedAnchor = false;     
        }

        protected void SetAnchorOffset(Vector2 offsetPosition){
            Vector2 offset = lastRestingPosition + offsetPosition;
            anchor.style.left = offset.x;
            anchor.style.top = offset.y;
            coordDisplay.text = "[" + (int)offset.x + "," + (int)offset.y + "]";
        }

        protected void SetAnchorPosition(Vector2 position){
            anchor.style.left = position.x;
            anchor.style.top = position.y;
            coordDisplay.text = "[" + (int)position.x + "," + (int)position.y + "]";
            lastRestingPosition = position;
        }

        protected void ResetDrag(){
            lastMousePosition = new Vector2(0,0);
            lastRestingPosition = new Vector2(0,0);
            anchor.style.left = 0;
            anchor.style.top = 0;
            coordDisplay.text = "[0,0]";
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        protected void ZoomWindow(WheelEvent e){
            float oldScale = anchor.style.scale.value.value.x ;
            float newScale = Mathf.Clamp(oldScale + (-e.delta.y * 0.05f), 0.1f, 1);
            float scaleChange = newScale - oldScale;
            AnchorItemPositionFromMouse(e, out Vector2 position);
            float x = -(position.x * scaleChange);
            float y = -(position.y * scaleChange);
            anchor.style.scale = new StyleScale(new Scale(new(newScale,newScale,newScale)));            
            SetAnchorPosition(new(anchor.style.left.value.value + x, anchor.style.top.value.value + y));
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        protected void FrameSelection(){            
            Rect windowRect = this.AbsoluteRect();
            Vector2 newPos = new();
            int count = 0;
            anchor.Query<Node<T>>(null,"selected").ForEach( node => {
                count++;
                Rect nodeRect = node.AbsoluteRect();
                newPos.x += -nodeRect.x - nodeRect.width/2;
                newPos.y += -nodeRect.y - nodeRect.height/2;
                
            });
            if(count == 0) return;
            newPos.x = (newPos.x/count * anchor.style.scale.value.value.x) + (windowRect.width/2);
            newPos.y = (newPos.y/count * anchor.style.scale.value.value.x) + (windowRect.height/2);
            SetAnchorPosition(newPos);
        }

        protected void GridSelection(){
            List<Node<T>> selectedNodes = new();
            anchor.Query<Node<T>>(null,"selected").ForEach( node => {selectedNodes.Add(node);});
            if(selectedNodes.Count == 0) return;
            Vector2 furthestOut = selectedNodes.First().PositionToVector2();

            foreach(Node<T> node in selectedNodes){
                Vector2 position = node.PositionToVector2();
                if(position.x < furthestOut.x) furthestOut.x = position.x;
                if(position.y < furthestOut.y) furthestOut.y = position.y;
            }

            selectedNodes = selectedNodes.OrderBy(node => node.PositionToVector2().y).ThenBy(node => node.PositionToVector2().x).ToList();
            int size = Mathf.CeilToInt(Mathf.Sqrt(selectedNodes.Count));
        
            int index = 0;
            for(int y = 0; y < size; y++){
                for(int x = 0; x < size; x++){
                    if(index < selectedNodes.Count){
                        Vector2 newPosition = new(){
                            x = furthestOut.x + gridSize.x * x,
                            y = furthestOut.y + gridSize.y * y
                        };
                        selectedNodes[index].style.left = newPosition.x;
                        selectedNodes[index].style.top = newPosition.y;
                        selectedNodes[index].lastRestingPosition = newPosition;
                    }                    
                    index++;
                }
            }
        }

        protected void ColumnSelection(){
            List<Node<T>> selectedNodes = new();
            anchor.Query<Node<T>>(null,"selected").ForEach( node => {selectedNodes.Add(node);});
            if(selectedNodes.Count == 0) return;
            Vector2 furthestOut = selectedNodes.First().PositionToVector2();

            foreach(Node<T> node in selectedNodes){
                Vector2 position = node.PositionToVector2();
                if(position.x < furthestOut.x) furthestOut.x = position.x;
                if(position.y < furthestOut.y) furthestOut.y = position.y;
            }
            
            selectedNodes = selectedNodes.OrderBy(node => node.PositionToVector2().y).ThenBy(node => node.PositionToVector2().x).ToList();

            for(int y = 0; y < selectedNodes.Count; y++){
                Vector2 newPosition = new(){
                    x = furthestOut.x,
                    y = furthestOut.y + gridSize.y * y
                };
                selectedNodes[y].style.left = newPosition.x;
                selectedNodes[y].style.top = newPosition.y;
                selectedNodes[y].lastRestingPosition = newPosition;                    
            }
        }

        protected void RowSelection(){
            List<Node<T>> selectedNodes = new();
            anchor.Query<Node<T>>(null,"selected").ForEach( node => {selectedNodes.Add(node);});
            if(selectedNodes.Count == 0) return;
            Vector2 furthestOut = selectedNodes.First().PositionToVector2();

            foreach(Node<T> node in selectedNodes){
                Vector2 position = node.PositionToVector2();
                if(position.x < furthestOut.x) furthestOut.x = position.x;
                if(position.y < furthestOut.y) furthestOut.y = position.y;
            }
            
            selectedNodes = selectedNodes.OrderBy(node => node.PositionToVector2().x).ThenBy(node => node.PositionToVector2().y).ToList();

            for(int x = 0; x < selectedNodes.Count; x++){
                Vector2 newPosition = new(){
                    x = furthestOut.x + gridSize.x * x,
                    y = furthestOut.y 
                };
                selectedNodes[x].style.left = newPosition.x;
                selectedNodes[x].style.top = newPosition.y;
                selectedNodes[x].lastRestingPosition = newPosition;                    
            }
        }


         protected void AnchorItemPositionFromMouse(IMouseEvent e, out Vector2 position){
            Vector2 anchorPosition = anchor.PositionToVector2();
            position = new(){
                x = (e.localMousePosition.x * (1 / anchor.style.scale.value.value.x)) - (anchorPosition.x * (1 / anchor.style.scale.value.value.x)),
                y = (e.localMousePosition.y * (1 / anchor.style.scale.value.value.x)) - (anchorPosition.y * (1 / anchor.style.scale.value.value.x))
            };
        }
        
    }

}
