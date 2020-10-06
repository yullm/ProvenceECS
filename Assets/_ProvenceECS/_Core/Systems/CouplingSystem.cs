using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class Parent : ProvenceComponent{
        public HashSet<Entity> children;

        public Parent(){
            this.children = new HashSet<Entity>();
        }
    }

    public class Child : ProvenceComponent{
        public Entity parent;

        public Child(){
            this.parent = null;
        }

        public Child(Entity parent){
            this.parent = parent;
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
        
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<ComponentRemoved<Parent>>(ParentRemoved);
            world.eventManager.AddListener<ComponentRemoved<Child>>(ChildRemoved);
            world.eventManager.AddListener<ComponentAdded<Child>>(ChildAdded);
        }

        protected void ParentRemoved(ComponentRemoved<Parent> args){
            foreach(Entity child in args.handle.component.children){
                world.RemoveEntity(child);
            }
        }

        protected void RemoveChild(RemoveChild args){
            ComponentHandle<Parent> parentHandle = world.GetComponent<Parent>(args.parent);
            if(parentHandle != null && parentHandle.component.children.Contains(args.child)){
                parentHandle.component.children.Remove(args.child);
                world.RemoveEntity(args.child);
            }
        }

        protected void ChildAdded(ComponentAdded<Child> args){
            if(args.handle.component.parent != null){
                EntityHandle parentEntityHandle = world.LookUpEntity(args.handle.component.parent);
                if(parentEntityHandle != null){
                    ComponentHandle<Parent> parentHandle = parentEntityHandle.GetOrCreateComponent<Parent>();
                    parentHandle.component.children.Add(args.handle.entity);
                    GameObject parentObj = parentEntityHandle.AddGameObject();
                    GameObject childObj = world.AddGameObject(args.handle.entity);
                    childObj.transform.parent = parentObj.transform;
                }
            }
        }

        protected void ChildRemoved(ComponentRemoved<Child> args){
            ComponentHandle<Parent> parentHandle = world.GetComponent<Parent>(args.handle.component.parent);
            if(parentHandle != null) parentHandle.component.children.Remove(args.handle.entity);
            GameObject childObj = world.GetGameObject(args.handle.entity);
            childObj.transform.parent = null;
        }

        protected override void GatherCache(){
            throw new System.NotImplementedException();
        }

    }

}