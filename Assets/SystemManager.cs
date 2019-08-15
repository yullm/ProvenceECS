using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public class ProvenceSystem : ScriptableObject {
        public World world;
        public SystemManager manager;

        public virtual void Init(){}
    }

    public class SystemManager : MonoBehaviour
    {
        public World world;
        public List<ProvenceSystem> systems = new List<ProvenceSystem>();

        public void InitializeSystems(){
            foreach(ProvenceSystem system in systems){ 
                system.world = world;
                system.Init();
            }
        }

        public void AddSystem<T>() where T : ProvenceSystem{
            foreach(ProvenceSystem sys in systems){
                if(sys.GetType() == typeof(T)) return;
            }
            T system = ScriptableObject.CreateInstance<T>() as T;
            system.world = world;
            system.manager = this;
            systems.Add(system);
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