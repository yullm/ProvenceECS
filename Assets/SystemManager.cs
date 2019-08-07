using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    [System.Serializable]
    public class ProvenceSystem {}

    public class SystemManager : MonoBehaviour
    {
        public World world;
        public List<ProvenceSystem> systems = new List<ProvenceSystem>();  

        public void AddSystem<T>() where T : ProvenceSystem{
            T system = System.Activator.CreateInstance(typeof(T)) as T;
            systems.Add(system);
        }      
    }
}