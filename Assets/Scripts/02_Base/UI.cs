using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    Lobby,
}

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; protected set; }
    public abstract void OnOpened();
    public abstract void OnClosed();
}
