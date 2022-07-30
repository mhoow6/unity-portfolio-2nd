using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Mhoow6.SafeArea
{
    public abstract class SafeAreaTool : MonoBehaviour
    {
        public bool WarningMessage;

        protected RectTransform rectTransform;
        protected Canvas canvas;

        protected void Awake()
        {
            // ��ũ�� ������� safeArea�� ū ��� ����
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (screenSize.x < Screen.safeArea.size.x && screenSize.y < Screen.safeArea.size.y)
            {
                if (WarningMessage)
                    Debug.LogWarning($"��ũ�� ������� SafeArea�� ū ���, SafeArea Tool�� �����մϴ�.");
                enabled = false;
                return;
            }

            rectTransform = GetComponent<RectTransform>();

            if (TryGetCanvas(out var canvas))
                this.canvas = canvas;
            else
            {
                Canvas[] canvaslist = FindObjectsOfType<Canvas>();
                canvas = canvaslist.First(canvas =>
                {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        return true;
                    return false;
                });
            }

        }

        bool TryGetCanvas(out Canvas result)
        {            
            Canvas find = GameManager.UISystem.Canvas;
            result = find;

            if (result == null)
                return false;
            return true;
        }

        protected virtual void OnAwake() { }
    }
}

