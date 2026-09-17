using System;
using System.Collections.Generic;
using UnityEngine;

// Dictionary<TKey,TValue> isn't natively serializable by Unity - this flattens
// to parallel lists before serialize and rebuilds the dictionary after, so the
// rest of the codebase can use it exactly like a normal Dictionary.
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        int count = Mathf.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
            Add(keys[i], values[i]);
    }
}
