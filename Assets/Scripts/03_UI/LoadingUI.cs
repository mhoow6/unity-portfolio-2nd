using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class LoadingUI : UI
{
    [SerializeField] GameObject m_LoadingObject;
    [SerializeField] Slider m_LoadingSlider;
    [SerializeField] Text m_LoadingPercent;
    [SerializeField] Text m_RestTime;
    [SerializeField] GameObject m_LoadingCompleteObject;
    [SerializeField] Animator m_LoadingCompleteAnimator;

    public Action OnLoadComplete { get; set; }

    int m_downloadDataPerSecond;
    int m_needToLoadDataCount;
    int m_totalDownloadDataCount;
    int m_predictRestTime;

    public override UIType Type { get => UIType.Loading; }

    public void LoadingTitle(bool quickMode = false)
    {
        GameManager.Instance.LoadScene("LoadingTitle");

        m_LoadingObject.SetActive(true);
        m_LoadingCompleteObject.SetActive(false);

        // �ٿ�ε� �ؾ��� ������ �� ��������
        m_needToLoadDataCount = GameManager.Instance.Config.DownloadDataCount;
        m_LoadingSlider.maxValue = m_needToLoadDataCount;

        // �̺�Ʈ ����
        OnLoadComplete = () => { StartCoroutine(WaitForGameStart()); };

        if (quickMode)
            StartCoroutine(LoadComplete());
        else
        {
            StartCoroutine(FakeLoadingPercent());
            StartCoroutine(FakeRestTime());
        }
        
    }

    IEnumerator LoadComplete()
    {
        // �ε� �Ϸ������� �ؽ�Ʈ ����
        m_LoadingPercent.text = string.Format("{0:00.00}", 100);
        m_RestTime.text = "00:00";

        // �����̴� �� ���̰� �ϱ�
        m_LoadingSlider.gameObject.SetActive(false);

        // ���� �ʰ� �ε� �Ϸ� �� ������Ʈ �����ֱ�
        yield return new WaitForSeconds(0.5f);
        m_LoadingObject.SetActive(false);
        m_LoadingCompleteObject.SetActive(true);

        // �ε� �Ϸ�� ����� �ִϸ��̼��� ������ ��������
        if (m_LoadingCompleteAnimator != null)
            m_LoadingCompleteAnimator.SetBool("LoadingComplete", true);

        OnLoadComplete?.Invoke();
        OnLoadComplete = null;
    }

    IEnumerator FakeLoadingPercent()
    {
        var tick = new WaitForFixedUpdate();
        // �ε� ������ ���� ���� �ּҰ�, ���� �ִ밪
        int minValue = m_needToLoadDataCount > 20 ? 19 : m_needToLoadDataCount - 1;
        int maxValue = m_needToLoadDataCount > 20 ? 20 : m_needToLoadDataCount;
        while (m_totalDownloadDataCount < m_needToLoadDataCount)
        {
            m_downloadDataPerSecond = Random.Range(minValue, maxValue);
            m_totalDownloadDataCount += m_downloadDataPerSecond;
            m_LoadingSlider.value = m_totalDownloadDataCount;

            // �����
            float ratio = (float)m_totalDownloadDataCount / m_needToLoadDataCount;
            float percentage = ratio * 100;

            // 00.00%
            string totalDownloadByteText = string.Format("{0:00.00}", percentage);
            m_LoadingPercent.text = totalDownloadByteText;

            yield return tick;
        }
        yield return StartCoroutine(LoadComplete());
    }

    IEnumerator FakeRestTime()
    {
        var tick = new WaitForSeconds(0.5f);
        int timer = 0;
        while (true)
        {
            timer++;
            if (m_totalDownloadDataCount > 0)
            {
                int restDownloadByte = m_needToLoadDataCount - m_totalDownloadDataCount;
                // Debug.Log($"���� �ٿ�ε� �뷮: {restDownloadByte}");

                m_predictRestTime = restDownloadByte / m_downloadDataPerSecond;
                TimeSpan t = TimeSpan.FromSeconds(m_predictRestTime);
                // Debug.Log($"{t.Hours}, {t.Minutes}, {t.Seconds}");

                // 00:00
                string restTimeFormat = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                m_RestTime.text = restTimeFormat;
            }
            yield return tick;
        }
    }

    // �ε� �Ϸ� �� ���� ���� ���
    IEnumerator WaitForGameStart()
    {
        bool ready = false;
        while (!ready)
        {
            if (Input.GetMouseButton(0))
                ready = true;

            yield return null;
        }

        m_LoadingCompleteObject.SetActive(false);
        GameManager.Instance.LoadScene("MainMenu");
        GameManager.Instance.UISystem.CloseWindow();
        GameManager.Instance.UISystem.OpenWindow(UIType.MainMenu);
    }

    public override void OnOpened()
    {
        
    }

    public override void OnClosed()
    {
        
    }
}
