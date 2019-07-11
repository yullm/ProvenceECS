using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent{}

public class ComponentDictionary : SerializableDictionary<System.Type,Dictionary<Entity,IComponent>>{}

public class ComponentManager : MonoBehaviour{

    
    
}
