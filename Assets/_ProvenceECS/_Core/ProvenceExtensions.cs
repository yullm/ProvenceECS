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
            foreach(T item in items){
                list.Add(item);
            }
            return list;
        }

        public static List<T> Add<T>(this List<T> list, IEnumerable<T> items){
            foreach(T item in items){
                list.Add(item);
            }
            return list;
        }

        public static HashSet<T> Add<T>(this HashSet<T> set, IEnumerable<T> items){
            foreach(T item in items){
                set.Add(item);
            }
            return set;
        }

        public static List<T> Remove<T>(this List<T> list, IEnumerable<T> items){
            foreach(T item in items){
                list.Remove(item);
            }
            return list;
        }

        public static IEnumerable<T> Remove<T>(this HashSet<T> set, IEnumerable<T> items){
            foreach(T item in items){
                set.Remove(item);
            }
            return set;
        }

        public static List<T> Swap<T>(this List<T> list, int indexA, int indexB){
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
            return list;
        }

        public static bool SetEquals<T>(this List<T> list1, List<T> list2){
            return new HashSet<T>(list1).SetEquals(list2);
        }

        public static bool IsInRange(this int value, int min, int max){
            return value >= min && value <= max;
        }

        public static bool IsInRange(this float value, float min, float max){
            return value >= min && value <= max;
        }

    }

}