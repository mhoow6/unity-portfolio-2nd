using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericButton : LongClickButton
{
    public UnityEvent onButtonDown;
    public UnityEvent onButtonUp;

    protected override void OnButtonDown(PointerEventData eventData)
    {
        onButtonDown?.Invoke();
    }

    protected override void OnButtonUp(PointerEventData eventData)
    {
        onButtonUp?.Invoke();
    }
}
