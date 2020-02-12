using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ModifyComponentsSystem : ProvenceSystem{
        
        List<Type> componentTypes = new List<Type>();

        public override void Initialize(WorldRegistrationComplete args){
            componentTypes = Helpers.GetAllTypesFromBaseType(typeof(Component),typeof(Entity),typeof(UnityEngine.AI.NavMeshAgent));
            world.eventManager.AddListener<ModifyComponentEvent>(ModifyComponent);
        }

        public void ModifyComponent(ModifyComponentEvent args){
            if(args.entityToModify == null) {
                args.entityToModify = args.chosenComponent != null ? world.componentManager.GetEntityByComponent(args.chosenComponent).entity : args.conditionalEntity;
            }
            switch(args.modifyType){
                case ModifyComponentEvent.ModifyType.ADD:
                    AddComponent(args);
                    break;
                case ModifyComponentEvent.ModifyType.EDIT:
                    EditComponent(args);
                    break;
                case ModifyComponentEvent.ModifyType.REMOVE:
                case ModifyComponentEvent.ModifyType.REMOVE_ALL:
                    RemoveComponent(args);
                    break;
            }
            new EventHandle(world,args.OnCompleteCallback).Raise();
        }

        private void RemoveComponent(ModifyComponentEvent args){
            if(args.entityToModify != null){
                Type componentType = args.reflection.GetType();
                if(componentType != null){
                    if(args.modifyType == ModifyComponentEvent.ModifyType.REMOVE_ALL) 
                        world.componentManager.RemoveComponents(world.LookUpEntity(args.entityToModify),componentType);
                    else world.componentManager.RemoveComponent(world.LookUpEntity(args.entityToModify),componentType);
                    return;
                }
            }
        }
        
        private void EditComponent(ModifyComponentEvent args){
            if(args.entityToModify != null){
                Type componentType = args.reflection.GetType();
                if(componentType == null) return;
                if(world.componentManager.EntityHasComponent(args.entityToModify, componentType)){
                    FieldInfo[] fields = componentType.GetFields();
                    var component = args.chosenComponent;
                    if(component == null) component = world.componentManager.GetComponentByEntity(world.LookUpEntity(args.entityToModify),componentType).component;
                    for(int i = 0; i < fields.Length; i++){
                        try{
                            if(args.editStates[i])
                                fields[i].SetValue(component,fields[i].GetValue(args.reflection));
                            ((Behaviour)component).enabled = args.active;
                        }catch(Exception e){Debug.LogWarning(e);}
                    }
                }else{
                    if(args.editAddIfMissing) AddComponent(args);
                }
            }
        }

        private void AddComponent(ModifyComponentEvent args){
            if(args.entityToModify != null){
                Type componentType = args.reflection.GetType();
                if(componentType != null){
                    EntityHandle entityHandle = world.LookUpEntity(args.entityToModify);
                    var method = typeof(ComponentManager).GetMethod("AddComponent");
                    var reference = method.MakeGenericMethod(componentType);

                    FieldInfo[] fields = componentType.GetFields();

                    object[] fieldList = new object[fields.Length];
                    for(int i = 0; i < fieldList.Length; i++){
                        fieldList[i] = fields[i].GetValue(args.reflection);
                    }
                    reference.Invoke(world.componentManager, new object[]{entityHandle,fieldList});
                }else{
                    Debug.Log("Trouble adding type: " + args.reflection.GetType());
                }
            }
        }

        public override void GatherCache(){
            throw new System.NotImplementedException();
        }

        public override void IntegrityCheck(CacheIntegrityChange args){
            throw new System.NotImplementedException();
        }

    }

}