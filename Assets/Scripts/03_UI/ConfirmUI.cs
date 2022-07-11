using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConfirmUI : UI
{
    public RectTransform RectTransform;
    public Text MessageText;
    Action m_ConfirmCallback;
    Action m_CancelCallback;

    public override UIType Type => UIType.Confirm;

    public void SetData(string message, Action confirmCallback, Action cancelCallback = null)
    {
        MessageText.text = message;
        m_ConfirmCallback = confirmCallback;
        m_CancelCallback = cancelCallback;
    }

    public void OnConfirmBtnClick()
    {
        m_ConfirmCallback?.Invoke();
    }

    public void OnCancelBtnClick()
    {
        m_CancelCallback?.Invoke();
        GameManager.UISystem.CloseWindow(false);
    }

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        float tweenSpeed = GameManager.UISystem.SCALE_TWEENING_SPEED;
        RectTransform.localScale = new Vector3(1, 0, 1);
        RectTransform.DOScaleY(1f, tweenSpeed);
    }
}
