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

                //Debug.Log($"ĵ������ �ʺ�(����): {fullscreen_width}");
                //Debug.Log($"ĵ������ ����(�ø�): {fullscreen_height}");

                float safeAreaWidth = fullscreen_width * safeArea_width_ratio;
                float safeAreaHeight = fullscreen_height * safeArea_height_ratio;

                // Rect�� pivot�� (0,0)�ε� �ݸ� rectTransfom�� (0.5f, 0.5f)�̶� �������Ͱ� �ٸ���.
                // ���� _rectTransform.pivot.x�� ������� �ùٸ� x��ǥ�� ���� �� �ִ�.
                float safeAreaXCoordinate = fullscreen_width * safeArea_screen_diff_ratio * rectTransform.pivot.x;

                rectTransform.anchoredPosition = new Vector2(safeAreaXCoordinate, 0);
                rectTransform.sizeDelta = new Vector2(safeAreaWidth, safeAreaHeight);
            }
        }
    }
}

