using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainLobby,
    LoadingTitle,
    Adventure,
    NickNameInput,
    Warning,
    Confirm,
    ReadyForBattle,
    InGame,
    Sortie,
    Logo,
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
