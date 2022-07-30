using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mhoow6.SafeArea
{
    public class AutoExtendWidth : SafeAreaTool
    {
        void Start()
        {
            if (canvas)
            {
                var canvasRectTransform = canvas.GetComponent<RectTransform>();

                // ĵ���� RectTransform.sizeDelta�� ����̽��� ũ�⿡ ���缭 �˾Ƽ� �þ��.
                float fullscreen_width = Mathf.Floor(canvasRectTransform.sizeDelta.x);

                rectTransform.sizeDelta = new Vector2(fullscreen_width, rectTransform.sizeDelta.y);
            }
        }
    }
}

