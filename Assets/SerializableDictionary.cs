using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializableDictionary<K,V> : Dictionary<K,V>, ISerializationCallbackReceiver
{
    public List<K> keys = new List<K>();
    public List<V> values = new List<V>();

    public void OnBeforeSerialize()
    {   
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<K,V> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }

    }

    public void OnAfterDeserialize()
    {
        Clear();

        for (int i = 0; i != Mathf.Min(keys.Count, values.Count); i++)
            this.Add(keys[i], values[i]);
    }

}
