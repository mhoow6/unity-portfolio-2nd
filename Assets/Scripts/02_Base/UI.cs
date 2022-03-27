using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    MainMenu,
    Loading,
    Adventure,
    AskForQuit
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
