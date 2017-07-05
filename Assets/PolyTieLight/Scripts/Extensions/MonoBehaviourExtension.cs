using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public static class MonoBehaviourExtension 
{

    public static T GetInterface<T>(this MonoBehaviour inObj) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }

        return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }

    public static IEnumerable<T> GetInterfaces<T>(this MonoBehaviour inObj) where T : class
    {
        if (!typeof(T).IsInterface)
        {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return Enumerable.Empty<T>();
        }

        return inObj.GetComponents<Component>().OfType<T>();
    }
}
