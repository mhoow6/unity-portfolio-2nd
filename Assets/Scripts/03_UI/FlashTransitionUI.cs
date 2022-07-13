using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashTransitionUI : Toast
{
    public override ToastType Type => ToastType.FlashTransition;

    public CanvasGroup CanvasGroup;

    Action m_OnFadeInCallback;
    Action m_OnFadeOutCallback;
    float m_FadeTime;

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        m_OnFadeInCallback?.Invoke();
        StartCoroutine(FlashCoroutine());
    }

    public void SetData(Action onFadeInCallback, Action onFadeOutFallback, float fadeTime)
    {
        m_OnFadeInCallback = onFadeInCallback;
        m_OnFadeOutCallback = onFadeOutFallback;
        m_FadeTime = fadeTime;
    }

    public override void OnPushed()
    {
        Initalize = true;
    }

    IEnumerator FlashCoroutine()
    {
        float timer = 0f;
        float senstivity = 180 / m_FadeTime;
        bool m_Peek = false;
        float timerSenstivity = 1f;

        while (timer < m_FadeTime)
        {
            if (timer > m_FadeTime * 0.5f && m_Peek == false)
            {
                m_Peek = true;
                timerSenstivity = 4f;

                m_OnFadeOutCallback?.Invoke();
                m_OnFadeOutCallback = null;
            }

            timer += Time.deltaTime * timerSenstivity;

            CanvasGroup.alpha = Mathf.Sin(timer * senstivity * Mathf.Deg2Rad);

            yield return null;
        }
        GameManager.UISystem.CloseToast(true);
    }
}
