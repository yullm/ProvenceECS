using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace ProvenceECS{
    
    public static partial class ProvenceExtensions{

        public static void Raise<T>(this T pEvent, World world) where T : ProvenceEventArgs{
            world.eventManager.Raise(pEvent);
        }

        public static string SerializeObject<T>(this T obj){
            return JsonConvert.SerializeObject(obj, Formatting.None, Helpers.baseSerializerSettings);
        }

        public static T DeserializeObject<T>(this string data){
            return JsonConvert.DeserializeObject<T>(data,Helpers.baseSerializerSettings);
        }

        public static System.Object DeserializeObject(this string data){
            return JsonConvert.DeserializeObject(data,Helpers.baseSerializerSettings);
        }

        public static string NetSerializeObject<T>(this T obj){
            return JsonConvert.SerializeObject(obj, Formatting.None, Helpers.netSerializerSettings);
        }

        public static T NetDeserializeObject<T>(this string data){
            return JsonConvert.DeserializeObject<T>(data,Helpers.netSerializerSettings);
        }

        public static string LowCostSerializeObject<T>(this T obj){
            return JsonConvert.SerializeObject(obj, Formatting.None, Helpers.lowCostSerializerSettings);
        }

        public static T LowCostDeserializeObject<T>(this string data){
            return JsonConvert.DeserializeObject<T>(data,Helpers.lowCostSerializerSettings);
        }

        public static System.Object NetDeserializeObject(this string data){
            return JsonConvert.DeserializeObject(data,Helpers.netSerializerSettings);
        }

        public static Dictionary<Entity,ComponentHandle<BASE>> ToBaseDict<BASE,T>(this Dictionary<Entity,ComponentHandle<T>> dict) where T : BASE where BASE : ProvenceComponent{
            Dictionary<Entity,ComponentHandle<BASE>> newDict = new Dictionary<Entity, ComponentHandle<BASE>>();
            foreach(ComponentHandle<T> handle in dict.Values){
                newDict[handle.entity] = new ComponentHandle<BASE>(handle.entity,handle.component,handle.world);
            }
            return newDict;
        }

        public static HashSet<Entity> CacheIntersection(IEnumerable<Entity> mainCache, params IEnumerable<Entity>[] caches){
            HashSet<Entity> intersection = new HashSet<Entity>(mainCache);
            foreach(IEnumerable<Entity> cache in caches){
                intersection = new HashSet<Entity>(intersection.Intersect(cache));
            }
            return intersection;
        }
        
        /* public static Dictionary<Entity,ComponentHandle<T>> ToDictionary(this List<ComponentHandle<T>> list){
            Dictionary<Entity,ComponentHandle<ProvenceComponent>> dict = new Dictionary<Entity, ComponentHandle<ProvenceComponent>>();
            foreach(ComponentHandle<ProvenceComponent> handle in list){
                dict[handle.entity] = handle;
            }
            return dict;
        } */

        public static HashSet<ProvenceComponent> HandlesToComponents(this HashSet<ComponentHandle<ProvenceComponent>> inSet){
            HashSet<ProvenceComponent> outSet = new HashSet<ProvenceComponent>();
            foreach(ComponentHandle<ProvenceComponent> handle in inSet)
                outSet.Add(handle.component);
            return outSet;
        }

        public static Dictionary<Entity, HashSet<ProvenceComponent>> Clone(this Dictionary<Entity, HashSet<ProvenceComponent>> dict){
            Dictionary<Entity, Entity> entityCloneCache = new Dictionary<Entity, Entity>();
            Dictionary<ProvenceComponent, ProvenceComponent> componentCloneCache = new Dictionary<ProvenceComponent, ProvenceComponent>();
            Dictionary<Entity, HashSet<ProvenceComponent>> ecCloneCache = new Dictionary<Entity, HashSet<ProvenceComponent>>();

            foreach(KeyValuePair<Entity,HashSet<ProvenceComponent>> kvp in dict){
                Entity newEntity = new Entity();
                entityCloneCache[kvp.Key] = new Entity();
                ecCloneCache[newEntity] = new HashSet<ProvenceComponent>();
                foreach(dynamic component in kvp.Value){
                    ProvenceComponent newComponent = component.Clone();
                    componentCloneCache[component] = newComponent;
                    ecCloneCache[newEntity].Add(newComponent);
                }
            }

            foreach(HashSet<ProvenceComponent> set in ecCloneCache.Values){
                foreach(ProvenceComponent component in set){
                    FieldInfo[] fields = component.GetType().GetFields();
                    foreach(FieldInfo field in fields){
                        object fieldValue = field.GetValue(component);

                        if(fieldValue is Entity entity){
                            if(entityCloneCache.ContainsKey(entity))
                                field.SetValue(component, entityCloneCache[entity]);                        
                            continue;
                        }

                        if(fieldValue is ProvenceComponent fieldComponent){
                            if(componentCloneCache.ContainsKey(fieldComponent))
                                field.SetValue(component, componentCloneCache[fieldComponent]);
                        }
                    }
                }
            }
            
            return ecCloneCache;
        }

        public static HashSet<ProvenceComponent> Clone(this HashSet<ProvenceComponent> set){
            Dictionary<ProvenceComponent, ProvenceComponent> componentCloneCache = new Dictionary<ProvenceComponent, ProvenceComponent>();
            foreach(dynamic component in set){
                ProvenceComponent newComponent = component.Clone();
                componentCloneCache[component] = newComponent;
            }

            foreach(ProvenceComponent component in componentCloneCache.Values){
                FieldInfo[] fields = component.GetType().GetFields();
                foreach (FieldInfo field in fields){
                    object fieldValue = field.GetValue(component);

                    if(fieldValue is ProvenceComponent fieldComponent){
                        if(componentCloneCache.ContainsKey(fieldComponent))
                            field.SetValue(component, componentCloneCache[fieldComponent]);
                        else{
                            field.SetValue(component, fieldComponent.Clone());
                        }
                        continue;
                    }

                    // if(fieldValue is System.Collections.IEnumerable collection){
                    //     System.Type[] genericTypes = collection.GetType().GenericTypeArguments;
                    //     if(genericTypes.Length == 1 && collection.GetType().GenericTypeArguments[0].IsSubclassOf(typeof(ProvenceComponent))){

                    //     }
                    //     /* System.Type[] genericTypes = collection.GetType().GenericTypeArguments;
                    //     for(int i = 0; i < genericTypes.Length; i++){
                    //         if(genericTypes[i] == typeof(ProvenceComponent) || genericTypes[i].IsSubclassOf(typeof(ProvenceComponent))){ 
                    //             if(collection is System.Collections.IDictionary dict){

                    //             }else{
                    //                 if(i == 0){
                                        
                    //                 }
                    //             }
                    //         }
                    //     }  */
                    // }                               

                }
            }

            return new HashSet<ProvenceComponent>(componentCloneCache.Values);
        }

        public static void Clear(this GameObject go){
            try{
                for(int i = go.transform.childCount - 1; i >= 0; i--){
                    Object.DestroyImmediate(go.transform.GetChild(i).gameObject);
                }
            }catch(System.Exception e){
                Debug.LogWarning(e);
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
            foreach(T item in items.ToSet()){
                list.Remove(item);
            }
            return list;
        }

        public static IEnumerable<T> Remove<T>(this HashSet<T> set, IEnumerable<T> items){
            foreach(T item in items.ToSet()){
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

        public static HashSet<T> ToSet<T>(this IEnumerable<T> set){
            return new HashSet<T>(set);
        }

        public static void Log<T>(this IEnumerable<T> set){
            string setString = "";
            foreach(T item in set){
                setString += item.ToString() + ", ";
            }
            Debug.Log(setString);
        }

        public static bool SetEquals<T>(this List<T> list1, List<T> list2){
            return new HashSet<T>(list1).SetEquals(list2);
        }

        public static bool IsInRange(this int value, int a, int b){
            int min = Mathf.Min(a,b);
            int max = Mathf.Max(a,b);
            return value >= min && value <= max;
        }

        public static bool IsInRange(this float value, float a, float b){
            float min = Mathf.Min(a,b);
            float max = Mathf.Max(a,b);
            return value >= min && value <= max;
        }

        public static HashSet<Entity> DuplicateEntities(this World world, params Entity[] entities){
            //keep log of parent child relations and then check coupling components and replace, store GOs for ease
            Dictionary<Entity,Entity> clonePairs = new Dictionary<Entity, Entity>();
            HashSet<Entity> newEntities = new HashSet<Entity>();

            foreach(Entity entity in entities){
                ComponentHandle<Name> originalNameHandle = world.GetComponent<Name>(entity);
                EntityHandle duplicateHandle = world.CreateEntity();
                duplicateHandle.AddComponentSet(world.GetAllComponents(entity).HandlesToComponents().Clone());
                duplicateHandle.AddComponent<Name>(new Name(originalNameHandle.component.name + " copy"));
                newEntities.Add(duplicateHandle.entity);

                ComponentHandle<UnityGameObject> originalGOHandle = world.GetComponent<UnityGameObject>(entity);
                ComponentHandle<UnityGameObject> newGOHandle = world.GetComponent<UnityGameObject>(duplicateHandle.entity);
                if(originalGOHandle != null && newGOHandle != null){
                    if(originalGOHandle.component.gameObject != null && newGOHandle.component.gameObject != null){
                        newGOHandle.component.gameObject.transform.position = originalGOHandle.component.gameObject.transform.position;
                        newGOHandle.component.gameObject.transform.rotation = originalGOHandle.component.gameObject.transform.rotation;
                    }
                }
                clonePairs[entity] = duplicateHandle.entity;
            }

            HashSet<Entity> entitiesToSelect = new HashSet<Entity>(newEntities);

            foreach(Entity entity in newEntities){
                ComponentHandle<Child> childHandle = world.GetComponent<Child>(entity);

                if(childHandle != null){
                    Entity curParent = childHandle.component.parent;
                    if(clonePairs.ContainsKey(curParent)){
                        childHandle.component.parent = clonePairs[curParent];
                        entitiesToSelect.Remove(childHandle.entity);
                    }
                }
            }

            new EditorSelectEntities(world, entitiesToSelect).Raise(world);

            return newEntities;
        }

        public static void RemoveEntities(this World world, params Entity[] entities){
            foreach(Entity entity in entities){
                world.RemoveEntity(entity);
            }
        }

        public static Vector3 Snap(this Vector3 vector3, float gridSize = 1.0f){
            return new Vector3(
                Mathf.Round(vector3.x / gridSize) * gridSize,
                Mathf.Round(vector3.y / gridSize) * gridSize,
                Mathf.Round(vector3.z / gridSize) * gridSize
            );
        }
    }

}