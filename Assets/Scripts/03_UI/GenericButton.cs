using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericButton : LongClickButton
{
    public UnityEvent onButtonDown = new UnityEvent();
    public UnityEvent onButtonUp = new UnityEvent();
    public UnityEvent onButtonClick = new UnityEvent();

    protected override void OnButtonDown(PointerEventData eventData)
    {
        onButtonDown?.Invoke();
    }

    protected override void OnButtonUp(PointerEventData eventData)
    {
        onButtonUp?.Invoke();
    }

    protected override void OnButtonClick(PointerEventData eventData)
    {
        onButtonClick?.Invoke();
    }
}
