using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ProvenceECS.Mainframe{

    public class HierarchySelector : MainframeSelectorWindow<int[]>{

        private GameObject parent;
        private ScrollView scroller;

        public static HierarchySelector Open(){
            HierarchySelector.Close<HierarchySelector>();
            return HierarchySelector.GetWindow<HierarchySelector>();
        }

        protected override void SetEditorSettings(){
            this.titleContent = new GUIContent("Hierarchy Selector");
            this.uiKey = "hierarchy-selector";
        }

        protected override void RegisterEventListeners(){
            eventManager.AddListener<SetSelectorParameters<GameObject>>(DisplayHierarchy);
        }

        protected override void InitializeWindow(){
            if(root == null) return;
            scroller = root.Q<ScrollView>("list-scroller");
        }

        private void DisplayHierarchy(SetSelectorParameters<GameObject> args){
            if(args.paramaters == null) return;
            parent = args.paramaters;
            scroller.Clear();
            int[] depthPath = new int[]{};
            VisualElement rootShelf;
            VisualElement root = CreateObjectListItem(parent, out rootShelf, depthPath);
            scroller.Add(root);
            scroller.Add(rootShelf);
            DisplayChildren(parent, rootShelf, depthPath);
        }

        private void DisplayChildren(GameObject current, VisualElement shelf, int[] depthPath){
            for(int i = 0; i < current.transform.childCount; i++){
                List<int> newDepthPath = new List<int>(depthPath);
                newDepthPath.Add(i);

                GameObject child = current.transform.GetChild(i).gameObject;
                VisualElement childShelf;
                VisualElement childTitle = CreateObjectListItem(child, out childShelf, newDepthPath.ToArray());
                
                DisplayChildren(child,childShelf,newDepthPath.ToArray());
                shelf.Add(childTitle);
                if(child.transform.childCount > 0) shelf.Add(childShelf); 
            }
        }

        private VisualElement CreateObjectListItem(GameObject gameObject, out VisualElement shelf, int[] depthPath){

            VisualElement listItem = new VisualElement();
            listItem.AddToClassList("list-item");
            listItem.AddToClassList("hoverable");
            if(depthPath.Length > 0){
                if(depthPath.Length % 2 == 0)
                    if(depthPath[depthPath.Length - 1] % 2 == 0) listItem.AddToClassList("alternate");
                else
                    if(depthPath[depthPath.Length - 1] % 2 != 0) listItem.AddToClassList("alternate");
            }

            VisualElement curShelf = new VisualElement();
            curShelf.AddToClassList("list-item-shelf");
            
            for(int i = 1; i < depthPath.Length; i ++){
                VisualElement indent = new VisualElement();
                indent.AddToClassList("list-item-indent");
                listItem.Add(indent);
            }

            if(gameObject.transform.childCount > 0){

                TextElement toggle = new TextElement();
                toggle.AddToClassList("list-item-label");
                toggle.AddToClassList("second-alternate");
                toggle.AddToClassList("selectable");
                toggle.text = "v";
                toggle.RegisterCallback<MouseUpEvent>(e =>{
                    if(e.button != 0) return;
                    if(curShelf.style.display != DisplayStyle.None){ 
                        curShelf.style.display = DisplayStyle.None;
                        toggle.text = ">";
                    }else{ 
                        curShelf.style.display = DisplayStyle.Flex;
                        toggle.text = "v";
                    }
                });

                listItem.Add(toggle);
            
            }

            TextElement title = new TextElement();
            title.AddToClassList("list-item-text-display");
            title.AddToClassList("selectable");
            title.text = gameObject.name;
            title.RegisterCallback<MouseUpEvent>(e => {
                if(e.button != 0) return;
                if(listItem.ClassListContains("selected")){
                        ReturnSelection();
                }else{
                    chosenKey = depthPath;

                    VisualElement selected = root.Q<VisualElement>("","selected");
                    if(selected != null) selected.RemoveFromClassList("selected");
                    listItem.AddToClassList("selected");
                }
            });
            
            listItem.Add(title);
            shelf = curShelf;
            return listItem;
        }

        public static GameObject GetChildByHierarhcy(GameObject root, int[] hierarchy){
            GameObject current = root;
            foreach(int i in hierarchy){
                try{
                    current = current.transform.GetChild(i).gameObject;
                }catch(System.Exception e){
                    Debug.LogWarning(e);
                    return current;
                }
            }
            return current;
        }
       
    }

}