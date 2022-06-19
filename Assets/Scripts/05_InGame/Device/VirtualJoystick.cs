using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, InputProvider
{
    [ReadOnly] public int PointerId = -1;

    [SerializeField] RectTransform m_Background;

    [SerializeField] RectTransform m_Lever;
    Vector2 m_Input;

    float m_Radius;
    Vector2 m_Center;
    Quaternion m_BackgroundQuaternion;

    Vector2 InputProvider.Input => m_Input;

    public string DeviceName => "Joystick";

    private void Start()
    {
        // 레버를 정확하게 가운데로 맞추기 
        m_Lever.anchorMin = new Vector2(0.5f, 0.5f);
        m_Lever.anchorMax = new Vector2(0.5f, 0.5f);
        m_Lever.anchoredPosition = Vector2.zero;

        m_Radius = m_Background.rect.width * 0.5f;
        m_Center = m_Background.position;
        m_BackgroundQuaternion = m_Background.rotation;
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
        PointerId = eventData.pointerId;
        GameManager.InputSystem.ExternallyUsingFingerIds.Add(PointerId);
        m_Lever.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float distance = Vector2.Distance(m_Center, eventData.position);

        // 레버 이동
        Vector2 diff = eventData.position - m_Center;

        // 무조건 조이스틱 반지름 길이만큼 이동시키기 (느리게 이동하는 거 방지)
        m_Lever.position = (diff.normalized * m_Radius) + m_Center;

        // 조이스틱 백그라운드 이동
        Vector2 endPoint = new Vector2(m_Center.x, m_Center.y + m_Radius);
        Vector2 pivotAxis = (endPoint - m_Center).normalized;
        Vector2 leverPoint = new Vector2(m_Lever.position.x, m_Lever.position.y);
        Vector2 leverAxis = (leverPoint - m_Center).normalized;

        // 센터에서 레버로 향하는 벡터와 센터에서 수직인 벡터와의 각도
        float angle = Mathf.Abs(Vector3.Angle(pivotAxis, leverAxis));

        // 오른쪽으로 조이스틱을 옮길경우
        float deltaX = diff.x;
        if (deltaX > 0)
        {
            // 회전은 -로 해줘야한다.
            m_Background.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_BackgroundQuaternion.eulerAngles.z - angle));
        }
        // 왼쪽으로 조이스틱을 옮길경우
        else
        {
            // 회전은 +로 해줘야한다.
            m_Background.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_BackgroundQuaternion.eulerAngles.z + angle));
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        m_Lever.position = m_Background.position;
        m_Background.rotation = m_BackgroundQuaternion;
        
        GameManager.InputSystem.ExternallyUsingFingerIds.Remove(PointerId);
        PointerId = -1;
    }
}
