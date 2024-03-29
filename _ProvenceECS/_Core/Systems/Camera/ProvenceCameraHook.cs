﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceCameraHook : MonoBehaviour{
        
        public World world;
        public Entity entity;

        public void Init( World world, Entity entity){
            this.world = world;
            this.entity = entity;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination){
            if(world != null && entity != null){
                ImageEffectArgs args = new ImageEffectArgs(entity,source,destination);
                world.eventManager.Raise<ImageEffectArgs>(args);
                Graphics.Blit(args.source, args.destination);
            }
        }

    }

}