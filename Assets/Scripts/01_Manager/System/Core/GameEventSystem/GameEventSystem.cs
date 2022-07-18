using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem
{
    static List<IGameEventListener> listeners = new List<IGameEventListener>();
    static Queue<IGameEventListener> reserveRemovelisteners = new Queue<IGameEventListener>();

    public static void AddListener(IGameEventListener listener)
    {
        if (listeners.Contains(listener))
            return;

        listeners.Add(listener);

        while (reserveRemovelisteners.Count != 0)
            RemoveListener(reserveRemovelisteners.Dequeue());
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

        while (reserveRemovelisteners.Count != 0)
            RemoveListener(reserveRemovelisteners.Dequeue());
    }

    // params: 가변인수가 지정가능케 하는 키워드
    public static void SendEvent(GameEvent gameEvent, params object[] args)
    {
        foreach (var listener in listeners)
            listener.Listen(gameEvent, args);

        while (reserveRemovelisteners.Count != 0)
            RemoveListener(reserveRemovelisteners.Dequeue());
    }

    public static void ReserveRemoveListener(IGameEventListener listener)
    {
        reserveRemovelisteners.Enqueue(listener);
    }
}
