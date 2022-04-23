using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
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

    int m_DownloadDataPerSecond;
    int m_NeedToLoadDataCount;
    int m_TotalDownloadDataCount;
    int m_PredictRestTime;

    int m_FakeDataDownloadPerSecond;
    const int DATA_MAXIMUM_DOWNLOAD_PER_SECOND = 20;

    public override UIType Type { get => UIType.Loading; }

    IEnumerator FakeLoadingWithProgress()
    {
        var tick = new WaitForFixedUpdate();
        // �ε� ������ ���� ���� �ּҰ�, ���� �ִ밪
        m_FakeDataDownloadPerSecond = (int)(m_NeedToLoadDataCount * 0.01f) == 0 ? 1 : (int)(m_NeedToLoadDataCount * 0.01f);
        //Debug.Log($"�ʴ� ������ �ٿ�ε� ��: {m_FakeDataDownloadPerSecond}");

        int minValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND - 1 : m_FakeDataDownloadPerSecond - 1;
        int maxValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND : m_FakeDataDownloadPerSecond;
        //Debug.Log($"�ּ� ������ �ٿ�ε�: {minValue}");
        //Debug.Log($"�ּ� ������ �ٿ�ε�: {maxValue}");

        while (m_TotalDownloadDataCount < m_NeedToLoadDataCount)
        {
            m_DownloadDataPerSecond = Random.Range(minValue, maxValue);
            m_TotalDownloadDataCount += m_DownloadDataPerSecond;
            m_LoadingSlider.value = m_TotalDownloadDataCount;

            // �����
            float ratio = (float)m_TotalDownloadDataCount / m_NeedToLoadDataCount;
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
            if (m_TotalDownloadDataCount > 0)
            {
                int restDownloadByte = m_NeedToLoadDataCount - m_TotalDownloadDataCount;
                // Debug.Log($"���� �ٿ�ε� �뷮: {restDownloadByte}");

                m_PredictRestTime = restDownloadByte / m_DownloadDataPerSecond;
                TimeSpan t = TimeSpan.FromSeconds(m_PredictRestTime);
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
        m_NeedToLoadDataCount = GameManager.Instance.Config.DownloadDataCount;
        m_LoadingSlider.maxValue = m_NeedToLoadDataCount;

        // �̺�Ʈ ����
        OnLoadComplete += () => { StartCoroutine(WaitForGameStart()); };

        // �ε�Ÿ��Ʋ �������
        if (LoadingTitleMechanism.Instance != null)
            LoadingTitleMechanism.Instance.StartDirecting(this);

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
        GameManager.Instance.UISystem.CloseWindow();

        // �� ��ó�� ī�޶� �̵��ϴ� ���� ����
        if (MainMenuMechanism.Instance != null)
            MainMenuMechanism.Instance.StartDirecting();
    }
    #endregion

    public override void OnOpened()
    {
        
    }

    public override void OnClosed()
    {
        m_DownloadDataPerSecond = 0;
        m_NeedToLoadDataCount = 0;
        m_TotalDownloadDataCount = 0;
        m_PredictRestTime = 0;
        m_LoadingPercent.text = string.Format("{0:00.00}", 0);
        m_RestTime.text = "00:00";
        m_LoadingSlider.value = 0;
        IsLoadingComplete = false;
    }
}
