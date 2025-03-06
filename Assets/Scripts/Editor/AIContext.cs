using UnityEngine;
using System.Collections.Generic;

public class AIContext
{
    //public Character character;
    public Vector3 targetPosition;
    public float health;
    public Blackboard blackboard;

    public BTNode currentNode;
}


public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default;
    }
}