using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WarningUI : UI
{
    public Text MessageText;
    public RectTransform RectTransform;

    public override UIType Type => UIType.Warning;
    float m_TweeningSpeed = 0;
        
    public void SetData(string message)
    {
        MessageText.text = message;
    }

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        m_TweeningSpeed = GameManager.Instance.Config.UIScaleTweeningSpeed;
        RectTransform.localScale = new Vector3(1, 0, 1);
        RectTransform.DOScaleY(1f, m_TweeningSpeed);
        Invoke("AutoClose", 3f);
    }

    void AutoClose()
    {
        if (gameObject.activeSelf)
        {
            RectTransform.DOScaleY(0f, m_TweeningSpeed).OnComplete(() =>
            {
                GameManager.Instance.UISystem.CloseWindow();
            });
        }
    }
}
