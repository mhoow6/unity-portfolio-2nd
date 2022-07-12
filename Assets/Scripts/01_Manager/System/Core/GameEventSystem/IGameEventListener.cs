using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    None = 0,
    StageClear,
}

public interface IGameEventListener
{
    public void Listen(GameEvent gameEvent);
}
