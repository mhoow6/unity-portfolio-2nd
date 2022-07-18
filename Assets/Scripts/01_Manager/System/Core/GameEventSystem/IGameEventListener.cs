using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    None = 0,
    StageClear,
    StageFail,
    SwapCharacter,
}

public interface IGameEventListener
{
    public void Listen(GameEvent gameEvent);
    public void Listen(GameEvent gameEvent, params object[] args);
}
