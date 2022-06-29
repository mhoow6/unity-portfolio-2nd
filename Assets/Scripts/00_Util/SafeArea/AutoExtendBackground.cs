using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mhoow6.SafeArea
{
    public class AutoExtendBackground : SafeAreaTool
    {
        private void Start()
        {
            if (canavs)
            {
                var canvasRectTransform = canavs.GetComponent<RectTransform>();

                // 캔버스 RectTransform.sizeDelta는 디바이스의 크기에 맞춰서 알아서 늘어난다.
                float fullscreen_width = Mathf.Floor(canvasRectTransform.sizeDelta.x);
                float fullscreen_height = Mathf.Ceil(canvasRectTransform.sizeDelta.y);

                rectTransform.sizeDelta = new Vector2(fullscreen_width, fullscreen_height);
            }
        }
    }
}

