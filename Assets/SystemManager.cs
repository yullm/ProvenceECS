using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{
    [System.Serializable]
    public class ProvenceSystem : ScriptableObject {
        public World world;
    }

    public class SystemManager : MonoBehaviour
    {
        public World world;
        public List<ProvenceSystem> systems = new List<ProvenceSystem>();

        public void BroadcastWorld(){
            foreach(ProvenceSystem system in systems) system.world = world;
        }

        public void AddSystem<T>() where T : ProvenceSystem{
            foreach(ProvenceSystem sys in systems){
                if(sys.GetType() == typeof(T)) return;
            }
            T system = ScriptableObject.CreateInstance(typeof(T)) as T;
            systems.Add(system);
            system.world = world;
        }

        public void AddSystemByType(System.Type type){
            if(typeof(ProvenceSystem).IsAssignableFrom(type)){
                var method = typeof(SystemManager).GetMethod("AddSystem");
                var reference = method.MakeGenericMethod(type);
                reference.Invoke(this,null);
            }
        }
    }
}