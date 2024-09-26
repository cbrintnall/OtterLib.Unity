using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvent { }

public delegate bool EventDelegate<T>(T data);

public class EventSubscriptionRequest<T>
    where T : BaseEvent
{
    public EventDelegate<T> Callback;
    public EventDelegate<T> Precheck;

    public EventSubscriptionStorage ToStorage()
    {
        return new EventSubscriptionStorage() { Callback = Callback, Precheck = Precheck };
    }
}

public class EventSubscriptionStorage
{
    public Delegate Callback;
    public Delegate Precheck;
}

[Singleton(requiresScene: false)]
public class EventBus
{
    private Dictionary<Type, List<EventSubscriptionStorage>> subscribers = new();

    public static EventSubscriptionRequest<T> WithOwnershipPrecheck<T>(
        GameObject owner,
        EventDelegate<T> cb
    )
        where T : BaseEvent
    {
        return new EventSubscriptionRequest<T>() { Precheck = ev => owner != null, Callback = cb };
    }

    public static void Sub<T>(EventDelegate<T> cb)
        where T : BaseEvent =>
        SingletonLoader
            .Get<EventBus>()
            .Subscribe(new EventSubscriptionRequest<T>() { Callback = cb });

    public static void Sub<T>(EventSubscriptionRequest<T> req)
        where T : BaseEvent => SingletonLoader.Get<EventBus>().Subscribe(req);

    public static void Pub<T>(T data)
        where T : BaseEvent => SingletonLoader.Get<EventBus>().Publish(data);

    public void Subscribe<T>(EventSubscriptionRequest<T> request)
        where T : BaseEvent
    {
        if (!subscribers.ContainsKey(typeof(T)))
        {
            subscribers[typeof(T)] = new();
        }

        subscribers[typeof(T)].Add(request.ToStorage());
    }

    public void Publish<T>(T data)
        where T : BaseEvent
    {
        if (!subscribers.ContainsKey(typeof(T)))
            return;

        List<EventSubscriptionStorage> removals = new();
        foreach (var subscriber in subscribers[typeof(T)])
        {
            if (subscriber.Callback is EventDelegate<T> subscribed)
            {
                try
                {
                    if (
                        subscriber.Precheck != null
                        && !(subscriber.Precheck as EventDelegate<T>).Invoke(data)
                    )
                    {
                        removals.Add(subscriber);
                    }
                    else if (subscribed(data))
                    {
                        removals.Add(subscriber);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Subscribed threw exception {e.GetType()}, removing.");
                    Debug.LogError(e);
                    removals.Add(subscriber);
                }
            }
        }

        foreach (var removal in removals)
        {
            subscribers[typeof(T)].Remove(removal);
        }
    }
}
