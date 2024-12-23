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
            // 스크린 사이즈보다 safeArea가 큰 경우 제외
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (screenSize.x < Screen.safeArea.size.x && screenSize.y < Screen.safeArea.size.y)
            {
                if (WarningMessage)
                    Debug.LogWarning($"스크린 사이즈보다 SafeArea가 큰 경우, SafeArea Tool을 종료합니다.");
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

