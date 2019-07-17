using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceComponent : MonoBehaviour{}

    public class ComponentHandle<T> where T : ProvenceComponent{
        public Entity entity;
        public ProvenceComponent component;
        public World world;
    }

}