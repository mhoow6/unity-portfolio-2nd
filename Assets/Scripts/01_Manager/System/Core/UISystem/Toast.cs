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

    /// <summary> Toast�� Initalize�� true�϶��� ����˴ϴ�. </summary>
    public bool Initalize;

    /// <summary> Toast�� ���� �� ȣ���ϴ� �Լ� </summary>
    public abstract void OnOpened();

    /// <summary> Toast�� ���� �� ȣ���ϴ� �Լ� </summary>
    public abstract void OnClosed();

    /// <summary> Toast�� �ý��ۿ� ������ �� ȣ���ϴ� �Լ� </summary>
    public abstract void OnPushed();
}
