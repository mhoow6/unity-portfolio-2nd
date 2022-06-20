using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToastType
{
    Logo,
    SceneTransition,
}

public abstract class Toast : MonoBehaviour
{
    public abstract ToastType Type { get; }

    /// <summary> Toast는 Initalize가 true일때만 실행됩니다. </summary>
    public bool Initalize;

    /// <summary> Toast가 열릴 때 호출하는 함수 </summary>
    public abstract void OnOpened();

    /// <summary> Toast가 닫힐 때 호출하는 함수 </summary>
    public abstract void OnClosed();

    /// <summary> Toast를 시스템에 예약할 때 호출하는 함수 </summary>
    public abstract void OnPushed();
}
