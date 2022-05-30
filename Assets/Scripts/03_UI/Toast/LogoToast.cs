using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LogoToast : Toast
{
    [SerializeField] Image m_Logo;
    [SerializeField] CanvasGroup m_CanvasGroup;

    public override ToastType Type => ToastType.Logo;

    public override void OnClosed()
    {
        if (!GameManager.Instance.IsTestZone)
        {
            GameManager.Instance.LoadScene(
            SceneCode.Lobby,
            () =>
            {
                StartCoroutine(FadeOutCoroutine());
            },
            null,
            () =>
            {
                LobbyManager.Instance.Init();
            });
        }
        else
        {
            GameManager.Instance.LoadScene(
            SceneCode.Stage0000,
            () =>
            {
                StartCoroutine(FadeOutCoroutine());
            },
            null,
            () =>
            {
                StageManager.Instance.Init(() =>
                {
                    GameManager.UISystem.OpenWindow(UIType.InGame);
                });
            });
        }
    }

    public override void OnOpened()
    {
        gameObject.SetActive(true);

        m_Logo.color = new Color(m_Logo.color.r, m_Logo.color.g, m_Logo.color.b, 0);
        m_Logo.DOColor(new Color(m_Logo.color.r, m_Logo.color.g, m_Logo.color.b, 1), 2f)
            .OnComplete(() =>
            {
                m_Logo.DOColor(new Color(m_Logo.color.r, m_Logo.color.g, m_Logo.color.b, 0), 2f)
                .OnComplete(() =>
                {
                    GameManager.UISystem.CloseToast(false);
                });
            });
    }

    IEnumerator FadeOutCoroutine()
    {
        while (m_CanvasGroup.alpha > 0.05f)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 0, Time.deltaTime * 2f);
            yield return null;
        }
        m_CanvasGroup.alpha = 0f;

        gameObject.SetActive(false);
        GameManager.UISystem.CurrentToast = null;
    }
}
