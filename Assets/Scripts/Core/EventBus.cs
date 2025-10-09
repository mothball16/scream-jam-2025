using System.Collections.Generic;
using System;
// [!] This script was not made during the Scream Jam and was borrowed from a previous project.


/// <summary>
/// Provides a simple unsubscribe method (put your subscriptions into a list and dispose at the end)
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class EventConnection<T> : IDisposable
{
    private readonly Action<T> _action;
   
    public EventConnection(Action<T> action)
    {
        _action = action;
    }
    public void Dispose()
    {
        EventBus.Unsubscribe<T>(_action);
    }
}
/// <summary>
/// A simple thread-safe event bus implementation.
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    private static readonly Dictionary<Type, object> _locks = new();


    /// <summary>
    /// Attempts to get the lock of the specified event type. If no lock is found for that type,
    /// create one.
    /// </summary>
    /// <typeparam name="T">The event type</typeparam>
    /// <returns>The lock for the event type</returns>
    private static object GetLock<T>()
    {
        lock (_locks)
        {
            if (!_locks.TryGetValue(typeof(T), out var lockObj))
            {
                lockObj = new object();
                _locks[typeof(T)] = lockObj;
            }

            return lockObj;
        }
    }

    public static EventConnection<T> Subscribe<T>(Action<T> handler)
    {
        lock (GetLock<T>())
        {
            if (_subscribers.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Add(handler);
            }
            else
            {
                _subscribers[typeof(T)] = new() { handler };
            }
            return new EventConnection<T>(handler);
        }
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        lock (GetLock<T>())
        {
            if (_subscribers.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    _subscribers.Remove(typeof(T));
                }
            }
        }
    }

    public static void Publish<T>(T data)
    {
        List<Delegate> actions;
        lock (GetLock<T>())
        {
            if (!_subscribers.TryGetValue(typeof(T), out var mySubs))
                return;

            // we need a new list to avoid concurrent modification
            actions = new(mySubs);
        }

        foreach (var action in actions)
        {
            ((Action<T>)action)(data);
        }
    }
}