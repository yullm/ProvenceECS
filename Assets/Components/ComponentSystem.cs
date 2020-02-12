using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProvenceECS{

    public abstract class ComponentSystem : MonoBehaviour{
        public World world;
        public Entity entity;
    }

}