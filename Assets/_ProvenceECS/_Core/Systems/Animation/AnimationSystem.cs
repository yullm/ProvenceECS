using System.Collections;
using System.Collections.Generic;
using ProvenceECS.Network;
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
        public string eventName;

        public AnimationEvent(){
            this.entity = null;
            this.eventName = "";
        }

        public AnimationEvent(Entity entity, string eventName){
            this.entity = entity;
            this.eventName = eventName;
        }
    }


    [ProvencePacket(66)]
    public class PlayAnimation : ProvenceEventArgs{
        public Entity entity;
        public string animationKey;
        public float fadeTime;
        public string[] fallbackKeys;

        public PlayAnimation(){
            this.entity = null;
            this.animationKey = "";
            this.fadeTime = 0;
            this.fallbackKeys = new string[0];
        }

        public PlayAnimation(Entity entity, string animationKey, float fadeTime = 0.1f, params string[] fallbackKeys){
            this.entity = entity;
            this.animationKey = animationKey;
            this.fadeTime = fadeTime;
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

    public class AnimationSystem : ProvenceSystem{

        protected ComponentCache<Model> modelCache;

        public AnimationSystem(){
            this.modelCache = new ComponentCache<Model>();
        }        

        protected override void RegisterEventListeners(){            
            modelCache.StandardRegistration(world);
            
            world.eventManager.AddListener<PlayAnimation>(PlayAnimation);
            world.eventManager.AddListener<SetAnimationFloatParameter>(SetAnimationFloatParameter);
            world.eventManager.AddListener<SetAnimationIntParameter>(SetAnimationIntParameter);
        }

        protected override void DeregisterEventListeners(){
            modelCache.StandardDeregistration(world);

            world.eventManager.RemoveListener<PlayAnimation>(PlayAnimation);
            world.eventManager.RemoveListener<SetAnimationFloatParameter>(SetAnimationFloatParameter);
            world.eventManager.RemoveListener<SetAnimationIntParameter>(SetAnimationIntParameter);
        }

        protected void SetAnimationFloatParameter(SetAnimationFloatParameter args){
            try{
                if(modelCache.ContainsKey(args.entity)){
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
                if(modelCache.ContainsKey(args.entity)){
                    Animator animatorComponent = modelCache[args.entity].component.animatorComponent;
                    if(animatorComponent != null)
                        animatorComponent.SetInteger(args.key, args.value);
                }
            }catch{
                Debug.Log(args.entity + " does not have animation parameter: " + args.key);
            }
        }

        protected void PlayAnimation(PlayAnimation args){
            if(modelCache.ContainsKey(args.entity) && modelCache[args.entity].component.animatorComponent != null){
                Model model = modelCache[args.entity].component;
                string key = "";
                if(model.animationData.ContainsKey(args.animationKey)){
                    key = args.animationKey;                   
                }else{
                    for(int i = 0; i < args.fallbackKeys.Length; i++){
                        if(model.animationData.ContainsKey(args.fallbackKeys[i])){
                            key = args.fallbackKeys[i];
                            break;
                        }
                    }
                }
                if(!key.Equals("")){
                    string stateName = model.animationData[key].stateName;
                    if(!model.currentState.Equals(key)){
                        model.currentState = key;
                        model.animatorComponent.CrossFadeInFixedTime(stateName, args.fadeTime, model.animationData[key].layer);
                    }
                }                
            }
        }

    }

}