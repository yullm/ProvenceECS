using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProvenceECS{
    
    public static class ProvenceExtensions{
        
        /* public static Dictionary<Entity,ComponentHandle<T>> ToDictionary(this List<ComponentHandle<T>> list){
            Dictionary<Entity,ComponentHandle<ProvenceComponent>> dict = new Dictionary<Entity, ComponentHandle<ProvenceComponent>>();
            foreach(ComponentHandle<ProvenceComponent> handle in list){
                dict[handle.entity] = handle;
            }
            return dict;
        } */

        public static void Clear(this GameObject go){
            for(int i = go.transform.childCount - 1; i >= 0; i--){
                Object.DestroyImmediate(go.transform.GetChild(i).gameObject);
            }
        }

        public static void Sort<T,J>(this Dictionary<T,J> dict){
            try{
                SortedDictionary<T,J> sorted = new SortedDictionary<T, J>(dict);
                dict.Clear();
                foreach(KeyValuePair<T,J> pair in sorted){
                    dict[pair.Key] = pair.Value;
                }
            }catch(System.Exception e){
                Debug.LogWarning(e);
            }
        }

        public static List<T> Add<T>(this List<T> list, params T[] items){
            for(int i = 0; i < items.Length; i++){
                list.Add(items[i]);
            }
            return list;
        }

        public static List<T> Swap<T>(this List<T> list, int indexA, int indexB){
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
            return list;
        }

    }

}