using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    None = 0,
    StageClear,
    StageFail
}

public interface IGameEventListener
{
    public void Listen(GameEvent gameEvent);
}
