using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainMenu,
    Loading,
    Adventure,
    NickNameInput, // 미구현
    Equipment, // 미구현
    Shop, // 미구현
    Quest, // 미구현
    Warning, // 미구현
    Setting, // 미구현
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
