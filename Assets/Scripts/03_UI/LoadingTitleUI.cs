using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class LoadingTitleUI : UI
{
    [SerializeField] GameObject m_LoadingObject;
    [SerializeField] Slider m_LoadingSlider;
    [SerializeField] Text m_LoadingPercent;
    [SerializeField] Text m_RestTime;
    [SerializeField] GameObject m_LoadingCompleteObject;
    [SerializeField] Animator m_LoadingCompleteAnimator;

    [ReadOnly] public bool IsLoadingComplete;
    public Action OnLoadComplete { get; set; }

    int m_downloadDataPerSecond;
    int m_needToLoadDataCount;
    int m_totalDownloadDataCount;
    int m_predictRestTime;

    const int DATA_DOWNLOAD_MAXIMUM_SPEED = 40;

    public override UIType Type { get => UIType.Loading; }

    IEnumerator FakeLoadingWithProgress()
    {
        var tick = new WaitForFixedUpdate();
        // �ε� ������ ���� ���� �ּҰ�, ���� �ִ밪
        int minValue = m_needToLoadDataCount > DATA_DOWNLOAD_MAXIMUM_SPEED ? DATA_DOWNLOAD_MAXIMUM_SPEED - 1 : m_needToLoadDataCount - 1;
        int maxValue = m_needToLoadDataCount > DATA_DOWNLOAD_MAXIMUM_SPEED ? DATA_DOWNLOAD_MAXIMUM_SPEED : m_needToLoadDataCount;
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
        StartCoroutine(LoadComplete());
    }

    IEnumerator FakeLoadingWithTime()
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

    IEnumerator LoadComplete()
    {
        IsLoadingComplete = true;

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

    #region ����Ÿ��Ʋ �ε�
    public void LoadingTitle(bool quickMode = false)
    {
        IsLoadingComplete = false;

        m_LoadingObject.SetActive(true);
        m_LoadingCompleteObject.SetActive(false);

        // �ٿ�ε� �ؾ��� ������ �� ��������
        m_needToLoadDataCount = GameManager.Instance.Config.DownloadDataCount;
        m_LoadingSlider.maxValue = m_needToLoadDataCount;

        // �̺�Ʈ ����
        OnLoadComplete += () => { StartCoroutine(WaitForGameStart()); };

        // �ε�Ÿ��Ʋ �������
        GameManager.Instance.Mechanism_LoadingTitle.StartDirecting(this);

        if (quickMode)
            StartCoroutine(LoadComplete());
        else
        {
            StartCoroutine(FakeLoadingWithProgress());
            StartCoroutine(FakeLoadingWithTime());
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
        GameManager.Instance.System_UI.CloseWindow();

        // �� ��ó�� ī�޶� �̵��ϴ� ���� ����
        GameManager.Instance.Mechanism_MainMenu.StartDirecting();
    }
    #endregion

    public override void OnOpened()
    {
        
    }

    public override void OnClosed()
    {
        m_downloadDataPerSecond = 0;
        m_needToLoadDataCount = 0;
        m_totalDownloadDataCount = 0;
        m_predictRestTime = 0;
        m_LoadingPercent.text = string.Format("{0:00.00}", 0);
        m_RestTime.text = "00:00";
        m_LoadingSlider.value = 0;
        IsLoadingComplete = false;
    }
}
