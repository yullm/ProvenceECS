using System.Collections;
using System.Collections.Generic;
using ProvenceECS;
using UnityEngine;

namespace ProvenceECS.Mainframe{

    public class ModelLoaderSystem : ProvenceSystem{     

        public ModelLoaderSystem(){}

        protected override void RegisterEventListeners(){
            world.eventManager.AddListener<ComponentAddedEarly<Model>>(ModelAdded);
        }

        protected override void DeregisterEventListeners(){
            world.eventManager.RemoveListener<ComponentAddedEarly<Model>>(ModelAdded);
        }

        protected void ModelAdded(ComponentAddedEarly<Model> args){
            ProvenceManager.ModelBank.LoadModel(args.handle);
        }

    }

}