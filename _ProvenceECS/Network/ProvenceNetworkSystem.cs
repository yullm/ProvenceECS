using ProvenceECS.Mainframe;
using UnityEngine;

namespace ProvenceECS.Network{
    
    [ProvencePacket(1)]
    public class AddEntity : ProvenceEventArgs{

        public Entity entity;
        public string name;

        public AddEntity():base(){
            this.entity = "";
            this.name = "";
        }

        public AddEntity(Entity entity, string name = ""):base(){
            this.entity = entity;
            this.name = name;
        }

    }

    [ProvencePacket(2)]
    public class RemoveEntity : ProvenceEventArgs{

        public Entity entity;

        public RemoveEntity():base(){
            this.entity = "";
        }

        public RemoveEntity(Entity entity):base(){
            this.entity = entity;
        }

    }

    [ProvencePacket(3)]
    public class AddComponent : ProvenceEventArgs{
        public Entity entity;
        public ProvenceComponent component;

        public AddComponent(){
            this.entity = "";
            this.component = null;
        }

        public AddComponent(Entity entity, ProvenceComponent component){
            this.entity = entity;
            this.component = component;
        }
        
    }

    [ProvencePacket(4)]
    public class RemoveComponent : ProvenceEventArgs{
        public Entity entity;
        public ProvenceComponent component;

        public RemoveComponent(){
            this.entity = "";
            this.component = null;
        }

        public RemoveComponent(Entity entity, ProvenceComponent component){
            this.entity = entity;
            this.component = component;
        }
        
    }

    [ProvencePacket(8)]
    public class RemoveAllComponents : ProvenceEventArgs{
        public Entity entity;
        public System.Type[] exceptions;

        public RemoveAllComponents(){
            this.entity = "";
            this.exceptions = new System.Type[0];
        }

        public RemoveAllComponents(Entity entity, params System.Type[] exceptions){
            this.entity = entity;
            this.exceptions = exceptions;
        }
        
    }

    [ProvencePacket(5)]
    public class AddSystem : ProvenceEventArgs{
        public ProvenceSystem system;

        public AddSystem(){
            this.system = null;
        }

        public AddSystem(ProvenceSystem system){
            this.system = system;
        }
        
    }

    [ProvencePacket(7)]
    public class AddSystemPackage : ProvenceEventArgs{

        public string packageName;
    
        public AddSystemPackage(){}

        public AddSystemPackage(string packageName){
            this.packageName = packageName;
        }
    
    }

    public class ProvenceNetworkSystem : ProvenceSystem{
        protected override void RegisterEventListeners(){
            if(Application.isPlaying){
                world.eventManager.AddListener<AddEntity>(AddEntity);
                world.eventManager.AddListener<RemoveEntity>(RemoveEntity);
                world.eventManager.AddListener<AddComponent>(AddComponent);
                world.eventManager.AddListener<RemoveComponent>(RemoveComponent);
                world.eventManager.AddListener<RemoveAllComponents>(RemoveAllComponents);
                world.eventManager.AddListener<AddSystem>(AddSystem);
                world.eventManager.AddListener<AddSystemPackage>(AddSystemPackage);
            }
        }
    
        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<AddEntity>(AddEntity);
            world.eventManager.RemoveListener<RemoveEntity>(RemoveEntity);
            world.eventManager.RemoveListener<AddComponent>(AddComponent);
            world.eventManager.RemoveListener<RemoveComponent>(RemoveComponent);
            world.eventManager.RemoveListener<RemoveAllComponents>(RemoveAllComponents);
            world.eventManager.RemoveListener<AddSystem>(AddSystem);
            world.eventManager.RemoveListener<AddSystemPackage>(AddSystemPackage);            
        }

        protected void AddEntity(AddEntity args){
            if(args.entity.Equals("")) world.CreateEntity(args.name);
            else world.AddEntity(args.entity, args.name);
        }
        protected void RemoveEntity(RemoveEntity args){
            world.RemoveEntity(args.entity);
        }

        protected void AddComponent(AddComponent args){
            EntityHandle entityHandle = world.LookUpEntity(args.entity);
            if(entityHandle == null) entityHandle = world.AddEntity(args.entity);
            entityHandle.AddComponent((dynamic)args.component);
        }

        protected void RemoveComponent(RemoveComponent args){
            world.RemoveComponent(args.entity,(dynamic)args.component);
        }

        protected void RemoveAllComponents(RemoveAllComponents args){
            world.componentManager.RemoveAllComponents(args.entity,args.exceptions);
        }

        protected void AddSystem(AddSystem args){
            world.AddSystem((dynamic)args.system);
        }

        protected void AddSystemPackage(AddSystemPackage args){
            world.systemManager.AddSystemPackageAtRuntime(args.packageName);
        }
    
    }

}