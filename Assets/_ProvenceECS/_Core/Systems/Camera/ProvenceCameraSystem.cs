using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ProvenceECS.Mainframe;
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
        public Camera camera;
    }

    public class MainCamera : ProvenceComponent{}

    public class ProvenceCameraSystem : ProvenceSystem{

        protected Material bwMat;
        protected ProvenceAsset<Shader> bwShader;
    
        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<ComponentAdded<ProvenceCamera>>(CameraAdded);                        
            world.eventManager.AddListener<ComponentAdded<MainCamera>>(MainCameraAdded);
            world.eventManager.AddListener<ComponentAdded<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentAdded);
            world.eventManager.AddListener<ComponentRemoved<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentRemoved);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<ComponentAdded<ProvenceCamera>>(CameraAdded);                                    
            world.eventManager.RemoveListener<ComponentAdded<MainCamera>>(MainCameraAdded);
            world.eventManager.RemoveListener<ComponentAdded<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentAdded);
            world.eventManager.RemoveListener<ComponentRemoved<BlackAndWhiteImageEffect>>(BlackAndWhiteComponentRemoved);
        }

        public override void Awaken(WakeSystemEvent args){
            bwShader = new ProvenceAsset<Shader>("BWShader");
            bwMat = new Material(bwShader.asset);
        }

        protected void MainCameraAdded(ComponentAdded<MainCamera> args){
            ComponentHandle<ProvenceCamera> cameraHandle = world.GetOrCreateComponent<ProvenceCamera>(args.handle.entity);
            if(cameraHandle.component.camera == null) cameraHandle = world.AddComponent<ProvenceCamera>(args.handle.entity);
            cameraHandle.component.camera.gameObject.tag = "MainCamera";          
            cameraHandle.component.camera.gameObject.SetActive(true);            
            foreach(ComponentHandle<MainCamera> mainHandle in world.componentManager.GetAllComponents<MainCamera>()){
                if(mainHandle.entity != args.handle.entity){
                    ProvenceCamera pCamera = world.GetComponent<ProvenceCamera>(mainHandle.entity).component;
                    pCamera.camera.tag = "Untagged";
                    pCamera.camera.gameObject.SetActive(false);
                    world.RemoveComponent<MainCamera>(mainHandle.entity);
                }
            }
        }

        protected void CameraAdded(ComponentAdded<ProvenceCamera> args){
            ComponentHandle<UnityGameObject> objectHandle = world.GetOrCreateComponent<UnityGameObject>(args.handle.entity);
            if(objectHandle != null && objectHandle.component.gameObject != null){
                GameObject gameObject = objectHandle.component.gameObject;
                Camera camera = gameObject.GetComponent<Camera>();
                if(camera == null)
                    camera = gameObject.AddComponent<Camera>();
                args.handle.component.camera = camera;
                ProvenceCameraHook hook = gameObject.GetComponent<ProvenceCameraHook>();
                if(hook == null) hook = gameObject.AddComponent<ProvenceCameraHook>();
                hook.Init(args.handle.world, args.handle.entity);
            }
        }

        protected void BlackAndWhiteComponentAdded(ComponentAdded<BlackAndWhiteImageEffect> args){
            world.eventManager.AddListener<ImageEffectArgs>(BlackAndWhiteEffect);
        }

        protected void BlackAndWhiteComponentRemoved(ComponentRemoved<BlackAndWhiteImageEffect> args){
            world.eventManager.RemoveListener<ImageEffectArgs>(BlackAndWhiteEffect);
        }

        protected void BlackAndWhiteEffect(ImageEffectArgs args){
            RenderTexture temp = RenderTexture.GetTemporary(args.source.width, args.source.height, 0, args.source.format);
            Graphics.Blit(args.source, temp);
            Graphics.Blit(temp, args.source, bwMat);
            RenderTexture.ReleaseTemporary(temp);
        }
            
    }
}
