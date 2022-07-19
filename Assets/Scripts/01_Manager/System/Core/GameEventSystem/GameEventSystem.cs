using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem
{
    static List<IGameEventListener> listeners = new List<IGameEventListener>();
    static Queue<IGameEventListener> removeListenersQueue = new Queue<IGameEventListener>();

    public static void AddListener(IGameEventListener listener)
    {
        if (listeners.Contains(listener))
            return;

        listeners.Add(listener);

        while (removeListenersQueue.Count != 0)
            RemoveListener(removeListenersQueue.Dequeue());
    }

    public static void RemoveListener(IGameEventListener listener)
    {
        if (!listeners.Contains(listener))
            return;

        listeners.Remove(listener);
    }

    public static void SendEvent(GameEvent gameEvent)
    {
        foreach (var listener in listeners)
            listener.Listen(gameEvent);

        while (removeListenersQueue.Count != 0)
            RemoveListener(removeListenersQueue.Dequeue());
    }

    // params: 가변인수가 지정가능케 하는 키워드
    public static void SendEvent(GameEvent gameEvent, params object[] args)
    {
        foreach (var listener in listeners)
            listener.Listen(gameEvent, args);

        while (removeListenersQueue.Count != 0)
            RemoveListener(removeListenersQueue.Dequeue());
    }

    public static void LateRemoveListener(IGameEventListener listener)
    {
        removeListenersQueue.Enqueue(listener);
    }

    public static void Clear()
    {
        listeners.Clear();
        removeListenersQueue.Clear();
    }
}
