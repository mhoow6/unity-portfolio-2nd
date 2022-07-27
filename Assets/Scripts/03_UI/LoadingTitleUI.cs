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
    [SerializeField] SplitSlider m_LoadingSlider;
    [SerializeField] Text m_LoadingPercent;
    [SerializeField] Text m_RestTime;
    [SerializeField] GameObject m_LoadingCompleteObject;
    [SerializeField] Animator m_LoadingCompleteAnimator;

    public override UIType Type { get => UIType.LoadingTitle; }

    Action m_OnGameStartCallback;
    Action m_OnQuarterLoadCallback;
    Action m_OnHalfLoadCallback;
    Action m_OnThreeQuarterLoadCallback;
    Action m_OnAlmostLoadCallback;


    [SerializeField, ReadOnly] bool m_IsLoadingComplete;
    int m_DownloadDataPerSecond;
    int m_NeedToLoadDataCount;
    int m_TotalDownloadDataCount;
    int m_PredictRestTime;
    int m_FakeDataDownloadPerSecond;
    const int DATA_MAXIMUM_DOWNLOAD_PER_SECOND = 5;

    public override void OnOpened(){}

    public void SetData(Action onGameStartCallback, Action onQuarterLoadCallback = null, Action onHalfLoadCallback = null, Action onThreeQuarterLoadCallback = null, Action onAlmostLoadCallback = null)
    {
        m_OnGameStartCallback = onGameStartCallback;
        m_OnQuarterLoadCallback = onQuarterLoadCallback;
        m_OnHalfLoadCallback = onHalfLoadCallback;
        m_OnThreeQuarterLoadCallback = onThreeQuarterLoadCallback;
        m_OnAlmostLoadCallback = onAlmostLoadCallback;

        m_IsLoadingComplete = false;

        m_LoadingObject.SetActive(true);
        m_LoadingCompleteObject.SetActive(false);

        // �ٿ�ε� �ؾ��� ������ �� ��������
        m_NeedToLoadDataCount = GameManager.GameDevelopSettings.DownloadDataCount;

        // �����̴� �� ����
        m_LoadingSlider.SetData(0, m_NeedToLoadDataCount);
        m_LoadingSlider.Value = 0;
        m_LoadingSlider.gameObject.SetActive(true);

        // �ε�Ÿ��Ʋ �������
        LobbyManager.Instance.MovingRoad.Move(() => { return m_IsLoadingComplete; });

        StartCoroutine(FakeLoadingWithProgress());
        StartCoroutine(FakeLoadingWithTime());
    }

    public override void OnClosed()
    {
        m_DownloadDataPerSecond = 0;
        m_NeedToLoadDataCount = 0;
        m_TotalDownloadDataCount = 0;
        m_PredictRestTime = 0;
        m_LoadingPercent.text = string.Format("{0:00.00}", 0);
        m_RestTime.text = "00:00";
        m_LoadingSlider.Value = 0;
        m_IsLoadingComplete = false;
    }

    IEnumerator FakeLoadingWithProgress()
    {
        var tick = new WaitForFixedUpdate();
        // �ε� ������ ���� ���� �ּҰ�, ���� �ִ밪
        m_FakeDataDownloadPerSecond = (int)(m_NeedToLoadDataCount * 0.01f) == 0 ? 1 : (int)(m_NeedToLoadDataCount * 0.01f);
        //Debug.Log($"�ʴ� ������ �ٿ�ε� ��: {m_FakeDataDownloadPerSecond}");

        int minValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND - 1 : m_FakeDataDownloadPerSecond - 1;
        int maxValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND : m_FakeDataDownloadPerSecond;

        while (m_TotalDownloadDataCount < m_NeedToLoadDataCount)
        {
            m_DownloadDataPerSecond = Random.Range(minValue, maxValue);
            m_TotalDownloadDataCount += m_DownloadDataPerSecond;
            m_LoadingSlider.Value = m_TotalDownloadDataCount;

            // ����� ���
            float ratio = (float)m_TotalDownloadDataCount / m_NeedToLoadDataCount;
            float percentage = ratio * 100;

            // �ݹ� ����
            if (percentage > 25 && m_OnQuarterLoadCallback != null)
            {
                m_OnQuarterLoadCallback.Invoke();
                m_OnQuarterLoadCallback = null;
                //Debug.Log($"25% �� ������ �͵� ����Ϸ�");
            }

            if (percentage > 50 && m_OnHalfLoadCallback != null)
            {
                m_OnHalfLoadCallback.Invoke();
                m_OnHalfLoadCallback = null;
                //Debug.Log($"50% �� ������ �͵� ����Ϸ�");
            }

            if (percentage > 75 && m_OnThreeQuarterLoadCallback != null)
            {
                m_OnThreeQuarterLoadCallback.Invoke();
                m_OnThreeQuarterLoadCallback = null;
                //Debug.Log($"75% �� ������ �͵� ����Ϸ�");
            }

            if (percentage > 95 && m_OnAlmostLoadCallback != null)
            {
                m_OnAlmostLoadCallback.Invoke();
                m_OnAlmostLoadCallback = null;
                //Debug.Log($"95% �� ������ �͵� ����Ϸ�");
            }

            // �ٿ�ε� �� ǥ�� (00.00%)
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
        m_IsLoadingComplete = true;

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

        GameManager.Initialized = true;

        StartCoroutine(WaitForGameStart());
    }

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
        GameManager.UISystem.CloseAllWindow();

        // �� ��ó�� ī�޶� �̵��ϴ� ���� ����
        m_OnGameStartCallback?.Invoke();
    }
}
