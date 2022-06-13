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

    /// <summary> Toast�� Initalize�� true�϶��� ����˴ϴ�. </summary>
    public bool Initalize { get; set; }

    public virtual void OnOpened() { }
    public virtual void OnClosed() { }  
}
