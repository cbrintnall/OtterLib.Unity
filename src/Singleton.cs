using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Singleton : Attribute
{
    public bool RequiresScene = true;

    public Singleton(bool requiresScene = true)
    {
        RequiresScene = requiresScene;
    }
}

public class SingletonLoader
{
    static Dictionary<Type, object> singletons = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void LoadBasicSingletons()
    {
        Debug.Log("Loading non-scene singletons");
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(
                type =>
                    type.GetCustomAttribute<Singleton>() != null
                    && !type.GetCustomAttribute<Singleton>().RequiresScene
            )
            .ForEach(type =>
            {
                Debug.Log(type);
                singletons[type] = Activator.CreateInstance(type);
                UnityEngine.Debug.Log($"Created singleton {type.Name}");
            });
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadSceneSingletons()
    {
        Debug.Log("Loading scene singletons");
        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(
                type =>
                    type.GetCustomAttribute<Singleton>() != null
                    && type.GetCustomAttribute<Singleton>().RequiresScene
            )
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
        if (singletons.TryGetValue(typeof(T), out var value))
        {
            return value as T;
        }

        return null;
    }
}
