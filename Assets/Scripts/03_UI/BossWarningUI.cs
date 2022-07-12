using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWarningUI : Toast
{
    public override ToastType Type => ToastType.BossWarning;
    public CanvasGroup CanvasGroup;

    [Range(0, 1)]
    public float BlinkSenstivity;

    const float BLINK_DURATION = 4.5f;
    Action m_OnAlarmEndCallback;

    public override void OnClosed()
    {
        m_OnAlarmEndCallback = null;
    }

    public override void OnOpened()
    {
        StartCoroutine(BlinkCoroutine(m_OnAlarmEndCallback));
    }

    public override void OnPushed()
    {
        Initalize = true;
    }

    public void SetData(Action onAlarmEndCallback)
    {
        m_OnAlarmEndCallback = onAlarmEndCallback;
    }

    IEnumerator BlinkCoroutine(Action onAlarmEndCallback)
    {
        float timer = 0f;
        float degree = 0f;

        while (timer < BLINK_DURATION)
        {
            timer += Time.deltaTime;

            degree += 0.5f * BlinkSenstivity;
            degree %= 90f;

            CanvasGroup.alpha = Mathf.Cos(degree);

            yield return null;
        }
        CanvasGroup.alpha = 0f;
        onAlarmEndCallback?.Invoke();
        GameManager.UISystem.CloseToast(true);
    }
}
