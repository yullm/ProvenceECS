using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProvenceECS.Network;
using Unity.VisualScripting;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class AnimationKey{
        public static implicit operator string(AnimationKey value){
            return value.GetType().Name;
        }
    }


    [ProvencePacket(65)]
    public class AnimationEvent : ProvenceEventArgs{
        public Entity entity;
        public string eventData;

        public AnimationEvent(){
            this.entity = null;
            this.eventData = "";
        }

        public AnimationEvent(Entity entity, string eventData){
            this.entity = entity;
            this.eventData = eventData;
        }
    }

    public class RegisterAnimationEventKey : ProvenceEventArgs{
        public string key;
        public Type type;

        public RegisterAnimationEventKey(string key, Type type){
            this.key = key;
            this.type = type;
        }
    }


    [ProvencePacket(66)]
    public class PlayAnimation : ProvenceEventArgs{
        public Entity entity;
        public string animationKey;
        public float fadeTime;
        public string[] fallbackKeys;
        public bool playSame;

        public PlayAnimation(){
            entity = null;
            animationKey = "";
            fadeTime = 0;
            fallbackKeys = new string[0];
            playSame = false;
        }

        public PlayAnimation(Entity entity, string animationKey, params string[] fallbackKeys) : this(){
            this.entity = entity;
            this.animationKey = animationKey;
            this.fallbackKeys = fallbackKeys;
        }

    }

    [ProvencePacket(67)]
    public class SetAnimationFloatParameter : ProvenceEventArgs{

        public Entity entity;        
        public string key;
        public float value;

        public SetAnimationFloatParameter(){
            this.entity = "";
            this.key = "";
            this.value = 0;
        }

        public SetAnimationFloatParameter(Entity entity, string key, float value){
            this.entity = entity;
            this.key = key;
            this.value = value;
        }
    
    }

    [ProvencePacket(68)]
    public class SetAnimationIntParameter : ProvenceEventArgs{

        public Entity entity;        
        public string key;
        public int value;

        public SetAnimationIntParameter(){
            this.entity = "";
            this.key = "";
            this.value = 0;
        }

        public SetAnimationIntParameter(Entity entity, string key, int value){
            this.entity = entity;
            this.key = key;
            this.value = value;
        }
    
    }

    public class SetAnimationBoolParameter : ProvenceEventArgs{

        public Entity entity;        
        public string key;
        public bool value;

        /* public SetAnimationBoolParameter(){
            this.entity = "";
            this.key = "";
            this.value = false;
        } */

        public SetAnimationBoolParameter(Entity entity, string key, bool value){
            this.entity = entity;
            this.key = key;
            this.value = value;
        }
    
    }

    public abstract class AnimationEventArgs : ProvenceEventArgs{
        public Entity entity;
        public List<string> data;

        public AnimationEventArgs() : base(){
            entity = null;
            data = null;
        }

    }

    public class AnimationSystem : ProvenceSystem{

        protected Dictionary<string,Type> eventLookup;
        protected ComponentCache<Model> modelCache;
        

        public AnimationSystem(){
            eventLookup = new();
            modelCache = new ComponentCache<Model>();
        }        

        protected override void RegisterEventListeners(){     
            if(Application.isPlaying){     
                modelCache.StandardRegistration(world);
                
                world.eventManager.AddListener<AnimationEvent>(AnimationEvent);
                world.eventManager.AddListener<RegisterAnimationEventKey>(RegisterAnimationEventKey);
                world.eventManager.AddListener<PlayAnimation>(PlayAnimation);
                world.eventManager.AddListener<SetAnimationFloatParameter>(SetAnimationFloatParameter);
                world.eventManager.AddListener<SetAnimationIntParameter>(SetAnimationIntParameter);
                world.eventManager.AddListener<SetAnimationBoolParameter>(SetAnimationBoolParameter);
            }
        }

        protected override void DeregisterEventListeners(){
            modelCache.StandardDeregistration(world);

            world.eventManager.RemoveListener<AnimationEvent>(AnimationEvent);
            world.eventManager.RemoveListener<RegisterAnimationEventKey>(RegisterAnimationEventKey);
            world.eventManager.RemoveListener<PlayAnimation>(PlayAnimation);
            world.eventManager.RemoveListener<SetAnimationFloatParameter>(SetAnimationFloatParameter);
            world.eventManager.RemoveListener<SetAnimationIntParameter>(SetAnimationIntParameter);
            world.eventManager.RemoveListener<SetAnimationBoolParameter>(SetAnimationBoolParameter);
        }

        protected void AnimationEvent(AnimationEvent args){
            List<string> eventParts = new(args.eventData.Split("-"));
            string key = eventParts[0];
            eventParts.Remove(key);
            if(eventLookup.ContainsKey(key)){
                Helpers.InvokeGenericMethod(this, "RaiseAnimationEvent", eventLookup[key], args.entity,eventParts);
            }
        }

        protected void RaiseAnimationEvent<T>(Entity entity, List<string> data) where T : AnimationEventArgs, new(){
            new T(){entity = entity, data = data}.Raise(world);
        }

        protected void RegisterAnimationEventKey(RegisterAnimationEventKey args){
            eventLookup[args.key] = args.type;
        }

        protected void SetAnimationFloatParameter(SetAnimationFloatParameter args){
            try{
                if(modelCache.TryGetValue(args.entity, out ComponentHandle<Model> modelHandle) && modelHandle.component.animatorParameters.Contains(args.key)){
                    Animator animatorComponent = modelCache[args.entity].component.animatorComponent;
                    if(animatorComponent != null)
                        animatorComponent.SetFloat(args.key, args.value);
                }
            }catch{
                Debug.Log(args.entity + " does not have animation parameter: " + args.key);
            }
        }

        protected void SetAnimationIntParameter(SetAnimationIntParameter args){
            try{
                if(modelCache.TryGetValue(args.entity, out ComponentHandle<Model> modelHandle) && modelHandle.component.animatorParameters.Contains(args.key)){
                    Animator animatorComponent = modelCache[args.entity].component.animatorComponent;
                    if(animatorComponent != null)
                        animatorComponent.SetInteger(args.key, args.value);
                }
            }catch{
                Debug.Log(args.entity + " does not have animation parameter: " + args.key);
            }
        }

        protected void SetAnimationBoolParameter(SetAnimationBoolParameter args){
            try{
                if(modelCache.TryGetValue(args.entity, out ComponentHandle<Model> modelHandle) && modelHandle.component.animatorParameters.Contains(args.key)){
                    Animator animatorComponent = modelCache[args.entity].component.animatorComponent;
                    if(animatorComponent != null)
                        animatorComponent.SetBool(args.key, args.value);
                }
            }catch{
                Debug.Log(args.entity + " does not have animation parameter: " + args.key);
            }
        }

        protected void PlayAnimation(PlayAnimation args){
            if(modelCache.ContainsKey(args.entity) && modelCache[args.entity].component.animatorComponent != null){
                Model model = modelCache[args.entity].component;
                string key = "";
                //if(model.animationData.ContainsKey(args.animationKey)){
                    key = args.animationKey;                   
                /* }else{
                    for(int i = 0; i < args.fallbackKeys.Length; i++){
                        if(model.animationData.ContainsKey(args.fallbackKeys[i])){
                            key = args.fallbackKeys[i];
                            break;
                        }
                    }
                } */
                if(!key.Equals("")){
                    //string stateName = model.animationData[key].stateName;
                    List<string> states = new(){
                        key,
                        args.fallbackKeys
                    };
                    foreach(string state in states){
                        int stateHash = Animator.StringToHash(state);
                        if(!args.playSame && model.animatorComponent.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash){
                            continue;
                        }//compare to args.key
                        if(model.animatorComponent.HasState(0,stateHash))
                            model.animatorComponent.CrossFadeInFixedTime(state, args.fadeTime, 0);
                    }
                }                
            }
        }

    }

}