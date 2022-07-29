using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheatButton : Display, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.EnableCheat)
            GameManager.UISystem.OpenWindow(UIType.Cheat);
    }
}
