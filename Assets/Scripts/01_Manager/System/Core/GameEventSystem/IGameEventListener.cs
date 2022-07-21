using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    None = 0,
    STAGE_Clear,
    STAGE_Fail,
    LOBBY_ShowCharacter,
    LOBBY_GainCharacter,
    LOBBY_ChangePartyPreset,
    LOBBY_LevelUpCharacter,
    GLOBAL_ReachMaxLevelCharacter
}

public interface IGameEventListener
{
    public void Listen(GameEvent gameEvent);
    public void Listen(GameEvent gameEvent, params object[] args);
}
