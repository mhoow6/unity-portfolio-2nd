using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainMenu,
    Loading,
    Adventure,
    NickNameInput,
    Equipment, // 미구현
    Shop, // 미구현
    Quest, // 미구현
    Warning,
    Setting, // 미구현
    Confirm,
    ReadyForBattle
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
