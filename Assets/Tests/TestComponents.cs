using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class TestComponents : MonoBehaviour{
        public bool boolean = false;
        public Vector3 vector = Vector3.one;
        public int x = 0;
        public float y = 0;
        public GameObject go;
        public Entity entity;
        public List<Entity> entities;
    }

}