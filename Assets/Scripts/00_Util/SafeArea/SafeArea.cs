using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mhoow6.SafeArea
{
    public class SafeArea : SafeAreaTool
    {
        void Start()
        {
            float safeArea_width_ratio = Screen.safeArea.width / Screen.width;
            float safeArea_height_ratio = Screen.safeArea.height / Screen.height;
            float safeArea_screen_diff_ratio = Screen.safeArea.x / Screen.width;

            if (canavs)
            {
                var canvasRectTransform = canavs.GetComponent<RectTransform>();

                float fullscreen_width = Mathf.Floor(canvasRectTransform.sizeDelta.x);
                float fullscreen_height = Mathf.Ceil(canvasRectTransform.sizeDelta.y);

                //Debug.Log($"캔버스의 너비(버림): {fullscreen_width}");
                //Debug.Log($"캔버스의 높이(올림): {fullscreen_height}");

                float safeAreaWidth = fullscreen_width * safeArea_width_ratio;
                float safeAreaHeight = fullscreen_height * safeArea_height_ratio;

                // Rect는 pivot이 (0,0)인데 반면 rectTransfom은 (0.5f, 0.5f)이라 기저벡터가 다르다.
                // 따라서 _rectTransform.pivot.x을 곱해줘야 올바른 x좌표를 구할 수 있다.
                float safeAreaXCoordinate = fullscreen_width * safeArea_screen_diff_ratio * rectTransform.pivot.x;

                rectTransform.anchoredPosition = new Vector2(safeAreaXCoordinate, 0);
                rectTransform.sizeDelta = new Vector2(safeAreaWidth, safeAreaHeight);
            }
        }
    }
}

