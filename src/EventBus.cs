using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvent { }

public delegate bool EventDelegate<T>(T data);

public class EventSubscriptionRequest<T>
    where T : BaseEvent
{
    public EventDelegate<T> Callback;

    public EventSubscriptionStorage ToStorage()
    {
        return new EventSubscriptionStorage() { Callback = Callback };
    }
}

public class EventSubscriptionStorage
{
    public Delegate Callback;
}

[Singleton(requiresScene: false)]
public class EventBus
{
    private Dictionary<Type, List<EventSubscriptionStorage>> subscribers = new();

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
                    if (subscribed(data))
                    {
                        removals.Add(subscriber);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Subscribed threw exception {e.GetType()}, removing.");
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
