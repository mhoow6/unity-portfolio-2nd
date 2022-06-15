using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToastType
{
    Logo,
}

public abstract class Toast : MonoBehaviour
{
    public abstract ToastType Type { get; }

    /// <summary> Toast는 Initalize가 true일때만 실행됩니다. </summary>
    public bool Initalize { get; set; }

    public abstract void OnOpened();
    public abstract void OnClosed();
}
