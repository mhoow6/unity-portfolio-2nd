using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;
using System;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class LoadingUI : MonoBehaviour
{
    public GameObject LoadingObject;
    public Slider LoadingSlider;
    public Text LoadingPercent;
    public Text RestTime;
    public GameObject LoadingCompleteObject;
    public Animator LoadingCompleteAnimator;

    public Action OnGameStart;

    int m_downloadDataPerSecond;
    int m_needToLoadDataCount;
    int m_totalDownloadDataCount;
    int m_predictRestTime;

    public void StartLoading()
    {
        LoadingObject.SetActive(true);
        LoadingCompleteObject.SetActive(false);

        // 게임매니저 실행
        var gm = new GameObject("DontDestroyOnLoad").AddComponent<GameManager>();

        // 다운로드 해야할 데이터 양 가져오기
        m_needToLoadDataCount = gm.Config.DownloadDataCount;
        LoadingSlider.maxValue = m_needToLoadDataCount;

        StartCoroutine(FakeLoadingPercent());
        StartCoroutine(FakeRestTime());
    }

    IEnumerator FakeLoadingPercent()
    {
        var tick = new WaitForFixedUpdate();
        // 로딩 연출을 위한 랜덤 최소값, 랜덤 최대값
        int minValue = m_needToLoadDataCount > 20 ? 19 : m_needToLoadDataCount - 1;
        int maxValue = m_needToLoadDataCount > 20 ? 20 : m_needToLoadDataCount;
        while (m_totalDownloadDataCount < m_needToLoadDataCount)
        {
            m_downloadDataPerSecond = Random.Range(minValue, maxValue);
            m_totalDownloadDataCount += m_downloadDataPerSecond;
            LoadingSlider.value = m_totalDownloadDataCount;

            // 백분율
            float ratio = (float)m_totalDownloadDataCount / m_needToLoadDataCount;
            float percentage = ratio * 100;

            // 00.00%
            string totalDownloadByteText = string.Format("{0:00.00}", percentage);
            LoadingPercent.text = totalDownloadByteText;

            yield return tick;
        }
        // 로딩 완료했으니 텍스트 수정
        LoadingPercent.text = string.Format("{0:00.00}", 100);
        RestTime.text = "00:00";

        // 슬라이더 안 보이게 하기
        LoadingSlider.gameObject.SetActive(false);

        // 조금 늦게 "터치하여 게임 시작" 보여주기
        yield return new WaitForSeconds(0.5f);
        LoadingObject.SetActive(false);
        LoadingCompleteObject.SetActive(true);

        // "터치하여 게임 시작" 애니메이션
        LoadingCompleteAnimator.SetBool("LoadingComplete", true);

        StartCoroutine(WaitForGameStart());
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
                // Debug.Log($"남은 다운로드 용량: {restDownloadByte}");

                m_predictRestTime = restDownloadByte / m_downloadDataPerSecond;
                TimeSpan t = TimeSpan.FromSeconds(m_predictRestTime);
                // Debug.Log($"{t.Hours}, {t.Minutes}, {t.Seconds}");

                // 00:00
                string restTimeFormat = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                RestTime.text = restTimeFormat;
            }
            yield return tick;
        }
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
        LoadingCompleteObject.SetActive(false);

        OnGameStart?.Invoke();
    }

    #region TEST
    IEnumerator LoadingPercentTest()
    {
        var tick = new WaitForFixedUpdate();
        while (m_totalDownloadDataCount < m_needToLoadDataCount)
        {
            int DOWNLOAD_BYTE = Random.Range(0, 2);
            m_totalDownloadDataCount += DOWNLOAD_BYTE;
            LoadingSlider.value = m_totalDownloadDataCount;

            // 00.00%
            string totalDownloadByteText = string.Format("{0:00.00}", m_totalDownloadDataCount);
            LoadingPercent.text = totalDownloadByteText;

            yield return tick;
        }
        LoadingPercent.text = string.Format("{0:00.00}", m_needToLoadDataCount);

        // 조금 늦게 "터치하여 게임 시작" 보여주기
        yield return new WaitForSeconds(0.5f);
        LoadingObject.SetActive(false);
        LoadingCompleteObject.SetActive(true);
    }
    IEnumerator RestTimeTest()
    {
        var tick = new WaitForSeconds(1f);
        int timer = 0;
        while (true)
        {
            timer++;
            if (m_totalDownloadDataCount > 0)
            {
                int downloadByOneSec = m_totalDownloadDataCount / timer;
                int restDownloadByte = m_needToLoadDataCount - m_totalDownloadDataCount;

                m_predictRestTime = restDownloadByte / downloadByOneSec;

                // 00:00
                string restTimeFormat = string.Format("{0:00:00}", m_predictRestTime);
                RestTime.text = restTimeFormat;
            }
            yield return tick;
        }
    }
    #endregion
}
