using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider LoadingSlider;
    public Text LoadingPercent;
    public Text RestTime;

    const int LOADING_DATA_BYTE = 100;

    void Start()
    {
        StartCoroutine(LoadingTest());
    }

    IEnumerator LoadingTest()
    {
        int totalDownloadByte = 0;
        while (totalDownloadByte < LOADING_DATA_BYTE)
        {
            int DOWNLOAD_BYTE = Random.Range(0, 5);
            totalDownloadByte += DOWNLOAD_BYTE;

            string totalDownloadByteText = string.Format("{0:00.00}", totalDownloadByte);

            // {0} : 00.00%
            LoadingPercent.text = totalDownloadByteText;

            yield return null;
        }
    }
}
