using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Singleton : Attribute { }

public class SingletonLoader
{
    static Dictionary<Type, Component> singletons = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadSingletons()
    {
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<Singleton>() != null)
            .ForEach(type =>
            {
                var singleton = new GameObject(type.Name);
                GameObject.DontDestroyOnLoad(singleton);

                singletons[type] = singleton.AddComponent(type);

                UnityEngine.Debug.Log($"Created singleton {type.Name}");
            });
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void LoadInitializeOnLoad()
    {
        GameObject[] toLoad = Resources.LoadAll<GameObject>("AutoInitialize");

        foreach (var next in toLoad)
        {
            var go = GameObject.Instantiate(next);
            GameObject.DontDestroyOnLoad(go);
        }
    }

    public static T Get<T>()
        where T : class
    {
        if (singletons.TryGetValue(typeof(T), out Component value))
        {
            return value as T;
        }

        return null;
    }
}
