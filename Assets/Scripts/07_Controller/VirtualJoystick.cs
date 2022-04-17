using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, Inputable
{
    [SerializeField] RectTransform m_Background;

    [SerializeField] RectTransform m_Lever;
    Vector2 m_Input;

    float m_Radius;
    Vector2 m_Center;

    Vector2 Inputable.Input => m_Input;

    private void Start()
    {
        // 레버를 정확하게 가운데로 맞추기 
        m_Lever.anchorMin = new Vector2(0.5f, 0.5f);
        m_Lever.anchorMax = new Vector2(0.5f, 0.5f);
        m_Lever.anchoredPosition = Vector2.zero;

        m_Radius = m_Background.rect.width * 0.5f;
        m_Center = m_Background.position;
    }

    private void Update()
    {
        // 중심점으로부터 얼만큼 떨어져있는지 정규화
        float x = 0f;
        float y = 0f;
        if (m_Lever.anchoredPosition.x != 0)
            x = m_Lever.anchoredPosition.x / m_Radius;
        if (m_Lever.anchoredPosition.y != 0)
            y = m_Lever.anchoredPosition.y / m_Radius;

        m_Input = new Vector2(x, y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_Lever.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float distance = Vector2.Distance(m_Center, eventData.position);
        //Debug.Log($"{distance}");

        Vector2 diff = eventData.position - m_Center;

        if (distance < m_Radius)
            m_Lever.position = (diff.normalized * distance) + m_Center;
        else
            m_Lever.position = (diff.normalized * m_Radius) + m_Center;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        m_Lever.position = m_Background.position;
    }
}
