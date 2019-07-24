using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceComponent : MonoBehaviour{}

    public class ComponentHandle<T> where T : Component{
        public Entity entity;
        public T component;
        public World world;

        public void Destroy(){
            world.LookUpEntity(entity).RemoveComponent<T>();
        }

    }



}