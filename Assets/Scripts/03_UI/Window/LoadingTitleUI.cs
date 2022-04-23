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
        // 로딩 연출을 위한 랜덤 최소값, 랜덤 최대값
        m_FakeDataDownloadPerSecond = (int)(m_NeedToLoadDataCount * 0.01f) == 0 ? 1 : (int)(m_NeedToLoadDataCount * 0.01f);
        //Debug.Log($"초당 데이터 다운로드 수: {m_FakeDataDownloadPerSecond}");

        int minValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND - 1 : m_FakeDataDownloadPerSecond - 1;
        int maxValue = m_FakeDataDownloadPerSecond > DATA_MAXIMUM_DOWNLOAD_PER_SECOND ? DATA_MAXIMUM_DOWNLOAD_PER_SECOND : m_FakeDataDownloadPerSecond;
        //Debug.Log($"최소 데이터 다운로드: {minValue}");
        //Debug.Log($"최소 데이터 다운로드: {maxValue}");

        while (m_TotalDownloadDataCount < m_NeedToLoadDataCount)
        {
            m_DownloadDataPerSecond = Random.Range(minValue, maxValue);
            m_TotalDownloadDataCount += m_DownloadDataPerSecond;
            m_LoadingSlider.value = m_TotalDownloadDataCount;

            // 백분율
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
        IsLoadingComplete = true;

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

        OnLoadComplete?.Invoke();
        OnLoadComplete = null;
    }

    #region 게임타이틀 로딩
    public void LoadingTitle(bool quickMode = false)
    {
        IsLoadingComplete = false;

        m_LoadingObject.SetActive(true);
        m_LoadingCompleteObject.SetActive(false);

        // 다운로드 해야할 데이터 양 가져오기
        m_NeedToLoadDataCount = GameManager.Instance.Config.DownloadDataCount;
        m_LoadingSlider.maxValue = m_NeedToLoadDataCount;

        // 이벤트 설정
        OnLoadComplete += () => { StartCoroutine(WaitForGameStart()); };

        // 로딩타이틀 연출시작
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

    // 로딩 완료 후 게임 시작 대기
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

        // 섬 근처로 카메라가 이동하는 연출 시작
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
