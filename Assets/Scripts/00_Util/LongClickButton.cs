using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    bool _pointerDown;
    float _pointerDownTimer;
    float _holdCycleTimer;

    public float requiredHoldTime;
    public float holdCycleTime;

    public UnityEvent onLongClick;


    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDown = true;

        OnButtonDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();

        OnButtonUp(eventData);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick(eventData);
    }

    protected virtual void OnButtonDown(PointerEventData eventData) { }
    protected virtual void OnButtonUp(PointerEventData eventData) { }
    protected virtual void OnButtonClick(PointerEventData eventData) { }

    void Update()
    {
        if (_pointerDown)
        {
            _pointerDownTimer += Time.deltaTime;
            if (_pointerDownTimer >= requiredHoldTime)
            {
                _holdCycleTimer += Time.deltaTime;
                if (_holdCycleTimer <= Time.deltaTime)
                {
                    onLongClick?.Invoke();
                }
                else if (_holdCycleTimer >= holdCycleTime)
                {
                    _holdCycleTimer = 0f;
                }

            }
        }
        else
            Reset();
    }

    void Reset()
    {
        _pointerDown = false;
        _pointerDownTimer = 0f;
        _holdCycleTimer = 0f;
    }

}
