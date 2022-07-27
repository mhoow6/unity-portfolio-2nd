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

        // 다운로드 해야할 데이터 양 가져오기
        m_NeedToLoadDataCount = GameManager.GameDevelopSettings.DownloadDataCount;

        // 슬라이더 값 조정
        m_LoadingSlider.SetData(0, m_NeedToLoadDataCount);
        m_LoadingSlider.Value = 0;
        m_LoadingSlider.gameObject.SetActive(true);

        // 로딩타이틀 연출시작
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
        // 로딩 연출을 위한 랜덤 최소값, 랜덤 최대값
        m_FakeDataDownloadPerSecond = (int)(m_NeedToLoadDataCount * 0.01f) == 0 ? 1 : (int)(m_NeedToLoadDataCount * 0.01f);
        //Debug.Log($"초당 데이터 다운로드 수: {m_FakeDataDownloadPerSecond}");

        int minValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND - 1 : m_FakeDataDownloadPerSecond - 1;
        int maxValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND : m_FakeDataDownloadPerSecond;

        while (m_TotalDownloadDataCount < m_NeedToLoadDataCount)
        {
            m_DownloadDataPerSecond = Random.Range(minValue, maxValue);
            m_TotalDownloadDataCount += m_DownloadDataPerSecond;
            m_LoadingSlider.Value = m_TotalDownloadDataCount;

            // 백분율 계산
            float ratio = (float)m_TotalDownloadDataCount / m_NeedToLoadDataCount;
            float percentage = ratio * 100;

            // 콜백 실행
            if (percentage > 25 && m_OnQuarterLoadCallback != null)
            {
                m_OnQuarterLoadCallback.Invoke();
                m_OnQuarterLoadCallback = null;
                //Debug.Log($"25% 때 실행할 것들 실행완료");
            }

            if (percentage > 50 && m_OnHalfLoadCallback != null)
            {
                m_OnHalfLoadCallback.Invoke();
                m_OnHalfLoadCallback = null;
                //Debug.Log($"50% 때 실행할 것들 실행완료");
            }

            if (percentage > 75 && m_OnThreeQuarterLoadCallback != null)
            {
                m_OnThreeQuarterLoadCallback.Invoke();
                m_OnThreeQuarterLoadCallback = null;
                //Debug.Log($"75% 때 실행할 것들 실행완료");
            }

            if (percentage > 95 && m_OnAlmostLoadCallback != null)
            {
                m_OnAlmostLoadCallback.Invoke();
                m_OnAlmostLoadCallback = null;
                //Debug.Log($"95% 때 실행할 것들 실행완료");
            }

            // 다운로드 량 표기 (00.00%)
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
                // Debug.Log($"남은 다운로드 용량: {restDownloadByte}");

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

        // 로딩 완료했으니 텍스트 수정
        m_LoadingPercent.text = string.Format("{0:00.00}", 100);
        m_RestTime.text = "00:00";

        // 슬라이더 안 보이게 하기
        m_LoadingSlider.gameObject.SetActive(false);

        // 조금 늦게 로딩 완료 시 오브젝트 보여주기
        yield return new WaitForSeconds(0.5f);
        m_LoadingObject.SetActive(false);
        m_LoadingCompleteObject.SetActive(true);

        // 로딩 완료시 출력할 애니메이션이 있으면 보여주자
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

        // 섬 근처로 카메라가 이동하는 연출 시작
        m_OnGameStartCallback?.Invoke();
    }
}
