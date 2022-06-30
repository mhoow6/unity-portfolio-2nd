using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mhoow6.SafeArea
{
    public abstract class SafeAreaTool : MonoBehaviour
    {
        protected RectTransform rectTransform;
        protected Canvas canavs;

        protected void Awake()
        {
            // PC���� �۵����� �ʵ��� ó��
            // ps. ��ũ�� ������� safeArea�� ū ���� PC�ۿ� ����
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (screenSize.x <= Screen.safeArea.size.x && screenSize.y <= Screen.safeArea.size.y)
            {
                Debug.LogWarning($"��ũ�� ������� SafeArea�� ū ���, SafeArea Tool�� �����մϴ�.");
                enabled = false;
                return;
            }

            rectTransform = GetComponent<RectTransform>();

            if (TryGetCanvas(out var canvas))
                canavs = canvas;
            else
            {
                Canvas[] canvaslist = FindObjectsOfType<Canvas>();
                canavs = canvaslist.First(canvas =>
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

