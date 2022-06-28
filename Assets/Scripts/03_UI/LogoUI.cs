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

        // �׽�Ʈ �� �����
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
        // �ΰ� ���̵� ��/�ƿ�
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
        // �̷��� �ϸ� �ٷ� ������.
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

        // LogoUI�� ���� �ִ� �ڵ�� �������� ����� �Ѵ�.
        gameObject.SetActive(false);
        GameManager.UISystem.CurrentToast = null;
    }
}
