using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LogoUI : Toast
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
                StartCoroutine(AutoCloseCoroutine());
            },
            null,
            () =>
            {
                LobbyManager.Instance.Init();
            });
            return;
        }

        // 테스트 존 입장시
        GameManager.Instance.LoadScene(
            SceneCode.Stage0000,
            () =>
            {
                gameObject.SetActive(false);
                GameManager.UISystem.CurrentToast = null;
                GameManager.UISystem.PushToast(ToastType.SceneTransition).Initalize = true;
            },
            null,
            () =>
            {
                StageManager.Instance.Init(() =>
                {
                    GameManager.UISystem.CloseToast(true);
                    GameManager.UISystem.OpenWindow(UIType.InGame);
                });
            });
    }

    public override void OnOpened()
    {
        // 로고 페이드 인/아웃
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

    public override void OnPushed()
    {
        // 이렇게 하면 바로 열린다.
        Initalize = true;
    }

    IEnumerator AutoCloseCoroutine()
    {
        while (m_CanvasGroup.alpha > 0.05f)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 0, Time.deltaTime * 2f);
            yield return null;
        }
        m_CanvasGroup.alpha = 0f;

        // LogoUI는 밑의 있는 코드로 수동으로 꺼줘야 한다.
        gameObject.SetActive(false);
        GameManager.UISystem.CurrentToast = null;
    }
}
