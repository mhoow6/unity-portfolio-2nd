using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mhoow6.SafeArea
{
    public class AutoExtendHeight : SafeAreaTool
    {
        private void Start()
        {
            if (canvas)
            {
                var canvasRectTransform = canvas.GetComponent<RectTransform>();

                // ĵ���� RectTransform.sizeDelta�� ����̽��� ũ�⿡ ���缭 �˾Ƽ� �þ��.
                float fullscreen_height = Mathf.Ceil(canvasRectTransform.sizeDelta.y);

                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fullscreen_height);
            }
        }
    }
}

