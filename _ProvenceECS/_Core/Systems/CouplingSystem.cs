using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ProvenceECS{


    public class Child : ProvenceComponent{
        public Entity parent;
        public Vector3? parentLastPos;
        public Quaternion? parentLastRot;

        public Vector3? lastPos;

        public Child(){
            this.parent = null;
            this.parentLastPos = null;
            this.parentLastRot = null;
        }

        public Child(Entity parent){
            this.parent = parent;
            this.parentLastPos = null;
            this.parentLastRot = null;
        }
    }

    public class RemoveChild : ProvenceEventArgs{
        public Entity parent;
        public Entity child;

        public RemoveChild(Entity parent, Entity child){
            this.parent = parent;
            this.child = child;
        }
    }

    public class CouplingSystem : ProvenceSystem{

        protected ComponentCache<UnityGameObject> ugoCache = new ComponentCache<UnityGameObject>();
        protected ComponentCache<Child> childrenCache = new ComponentCache<Child>();
        
        protected override void RegisterEventListeners(){
            /* world.eventManager.AddListener<ComponentRemoved<Parent>>(ParentRemoved);
            world.eventManager.AddListener<ComponentRemoved<Child>>(ChildRemoved); */
            //world.eventManager.AddListener<ComponentAdded<Child>>(ChildAdded);
            world.eventManager.AddListener<WorldUpdateEvent>(Update);
            world.eventManager.AddListener<EditorPersistanceUpdateEvent>(Update);

            ugoCache.StandardRegistration(world);
            childrenCache.StandardRegistration(world);
        }

        protected override void DeregisterEventListeners(){
            /* world.eventManager.RemoveListener<ComponentRemoved<Parent>>(ParentRemoved);
            world.eventManager.RemoveListener<ComponentRemoved<Child>>(ChildRemoved); */
            //world.eventManager.RemoveListener<ComponentAdded<Child>>(ChildAdded); 
            world.eventManager.RemoveListener<WorldUpdateEvent>(Update);
            world.eventManager.RemoveListener<EditorPersistanceUpdateEvent>(Update);
            
            ugoCache.StandardDeregistration(world);
            childrenCache.StandardDeregistration(world);
        }

        /* public override void Awaken(WakeSystemEvent args){
            foreach(Entity entity in childrenCache.Keys){
                InitializeParent(entity);
            }
        } */

        protected void Update(WorldUpdateEvent args){
            OffsetChildren();
        }

        protected void Update(EditorPersistanceUpdateEvent args){
            OffsetChildren();
        }

        protected void OffsetChildren(){
            foreach(ComponentHandle<Child> childHandle in childrenCache.Values){
                if(childHandle.component.parent == null) continue;
                if(ugoCache.ContainsKey(childHandle.component.parent) && ugoCache.ContainsKey(childHandle.entity)){
                    Child component = childHandle.component;
                    GameObject parentObject = ugoCache[component.parent].component.gameObject;
                    GameObject childObject = ugoCache[childHandle.entity].component.gameObject;

                    if(parentObject == null || childObject == null) continue;

                    if(component.parentLastPos == null || component.parentLastRot == null || component.lastPos == null)
                        InitializeParent(childHandle.entity);

                    Quaternion rotChange = parentObject.transform.rotation * Quaternion.Inverse((Quaternion)component.parentLastRot);
                    childObject.transform.rotation = rotChange * childObject.transform.rotation;
                    
                    childObject.transform.position = rotChange * (childObject.transform.position - (Vector3)component.parentLastPos) + (Vector3)component.parentLastPos;

                    #if UNITY_EDITOR
                    //if selection contains parent
                    if(Selection.objects.ToSet().Contains(parentObject.gameObject))
                    childObject.transform.position += parentObject.transform.position - (Vector3)component.parentLastPos - (childObject.transform.position - (Vector3)component.lastPos);
                    else
                    #endif
                    childObject.transform.position += parentObject.transform.position - (Vector3)component.parentLastPos /* - (childObject.transform.position - (Vector3)component.lastPos) */;
                    
                    component.parentLastPos = parentObject.transform.position;
                    component.parentLastRot = parentObject.transform.rotation;
                    component.lastPos = childObject.transform.position;
                }
            }
        }

        protected void InitializeParent(Entity entity){            
            //set inital last positions;
            ComponentHandle<Child> childHandle = childrenCache[entity];
            if(childHandle.component.parent != null && ugoCache.ContainsKey(childHandle.component.parent)){
                GameObject parentObject = ugoCache[childHandle.component.parent].component.gameObject;
                childHandle.component.parentLastPos = parentObject.transform.position;
                childHandle.component.parentLastRot = parentObject.transform.rotation;
                childHandle.component.lastPos = ugoCache[childHandle.entity].component.gameObject.transform.position;
            }
        }

        protected void ChildAdded(ComponentAdded<Child> args){
            /* childrenCache[args.handle.entity] = args.handle;
            InitializeParent(args.handle.entity); */
        }

    }

}