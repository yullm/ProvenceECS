using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{
    
    public class ImageEffectArgs : ProvenceEventArgs{
        
        public Entity cameraEntity;
        public RenderTexture source;
        public RenderTexture destination;

        public ImageEffectArgs(Entity cameraEntity, RenderTexture source, RenderTexture destination){
            this.cameraEntity = cameraEntity;
            this.source = source;
            this.destination = destination;
        }
    
    }

    public class BlackAndWhiteImageEffect : ProvenceComponent{}

    public class ProvenceCamera : ProvenceComponent{
        [JsonIgnore] public Camera camera;
        [JsonIgnore] public GameObject cameraObj;
    }

    public class ProvenceCameraSystem : ProvenceSystem{

        protected Material bwMat;
    
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<WakeSystemEvent>(Awaken);
            world.eventManager.AddListener<ComponentAdded<ProvenceCamera>>(CameraAdded);
            world.eventManager.AddListener<ComponentAdded<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentAdded);
            world.eventManager.AddListener<ComponentRemoved<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentRemoved);
        }

        public override void Awaken(WakeSystemEvent args){
            bwMat = new Material(Shader.Find("Meglo/Image Effects/BWShader"));
        }

        protected void CameraAdded(ComponentAdded<ProvenceCamera> args){
            GameObject gameObject = world.GetGameObject(args.handle.entity);
            if(gameObject != null){
                Camera camera = gameObject.GetComponent<Camera>();
                if(camera != null){
                    args.handle.component.camera = camera;
                    args.handle.component.cameraObj = gameObject;
                    ProvenceCameraHook hook = gameObject.GetComponent<ProvenceCameraHook>();
                    if(hook == null) hook = gameObject.AddComponent<ProvenceCameraHook>();
                    hook.Init(args.handle.world, args.handle.entity);
                }
            }
        }

        protected void BlackAndWhiteComponentAdded(ComponentAdded<BlackAndWhiteImageEffect> args){
            world.eventManager.RegisterStackMethod<ImageEffectArgs>(BlackAndWhiteEffect, 5, args.handle.entity);
        }

        protected void BlackAndWhiteComponentRemoved(ComponentRemoved<BlackAndWhiteImageEffect> args){
            world.eventManager.UnregisterStackMethod<ImageEffectArgs>(BlackAndWhiteEffect, args.handle.entity);
        }

        protected ImageEffectArgs BlackAndWhiteEffect(ImageEffectArgs args){
            RenderTexture temp = RenderTexture.GetTemporary(args.source.width, args.source.height, 0, args.source.format);
            Graphics.Blit(args.source, temp);
            Graphics.Blit(temp, args.source, bwMat);
            RenderTexture.ReleaseTemporary(temp);
            return args;
            
        }
        
        protected override void GatherCache(){}
    
    }
}
