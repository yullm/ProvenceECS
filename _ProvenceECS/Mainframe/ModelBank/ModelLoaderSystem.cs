using System.Collections;
using System.Collections.Generic;
using ProvenceECS;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class ModelLoaderSystem : ProvenceSystem{     

        public ModelLoaderSystem(){}

        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<ComponentAdded<Model>>(ModelAdded);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<ComponentAdded<Model>>(ModelAdded);
        }

        protected void ModelAdded(ComponentAdded<Model> args){
            ProvenceManager.ModelBank.LoadModel(args.handle);
        }

    }

}