using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProvenceECS{

    public enum ProvenceSystemCategory {INPUT,MENU,UNIT,INTERACTION,CAMERA,TEST};

    public abstract class ProvenceSystem : ScriptableObject {
        public World world;
        public bool cacheIsSafe = false;
        
        public abstract void Initialize(WorldRegistrationComplete args);

        public abstract void GatherCache();

        public abstract void IntegrityCheck(CacheIntegrityChange args);

    }

    public class SystemManager : MonoBehaviour{
        
        public World world;
        public List<ProvenceSystem> systems = new List<ProvenceSystem>();

        public void InitializeSystems(){
            foreach(ProvenceSystem system in systems){
                if(system == null) continue;
                system.world = world;
                world.eventManager.AddListener<WorldRegistrationComplete>(system.Initialize);
            }
        }

        public void AddSystem<T>() where T : ProvenceSystem{
            foreach(ProvenceSystem sys in systems){
                if(sys.GetType() == typeof(T)) return;
            }
            T system = ScriptableObject.CreateInstance<T>() as T;
            system.world = world;
            systems.Add(system);
            if(Application.isPlaying && system.world != null) system.Initialize(WorldRegistrationComplete.CreateInstance(world));
        }

        public ProvenceSystem GetSystem<T>() where T : ProvenceSystem{
            foreach(ProvenceSystem sys in systems){
                if(sys.GetType() == typeof(T)) return sys;
            }
            return null;
        }

        public void RemoveSystem<T>() where T : ProvenceSystem{
            foreach(ProvenceSystem system in systems){
                if(system.GetType() == typeof(T)){
                    systems.Remove(system);
                    return;
                }
            }
        }

        public void AddSystemByType(System.Type type){
            if(typeof(ProvenceSystem).IsAssignableFrom(type)){
                var method = typeof(SystemManager).GetMethod("AddSystem");
                var reference = method.MakeGenericMethod(type);
                reference.Invoke(this,null);
            }
        }

        public void OnDisable(){
            foreach(ProvenceSystem system in systems){
                Destroy(system);
            }
        }

    }
}