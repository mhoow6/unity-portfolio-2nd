using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToastType
{
    BlackFade,
    Logo,
}

public class Toast : MonoBehaviour
{
    public virtual ToastType Type { get; }

    /// <summary> Toast는 Initalize가 true일때만 실행됩니다. </summary>
    public bool Initalize { get; protected set; }

    public virtual void OnOpened() { }

    protected void Close(bool forceQuit = true)
    {
        OnClosed();
        if (forceQuit)
        {
            gameObject.SetActive(false);
            GameManager.UISystem.CurrentToast = null;
        }
            
    }
    protected virtual void OnClosed() { }  
}
