using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, InputProvider
{
    [ReadOnly] public int PointerId = -1;

    RectTransform m_JoystickRectTransfom;
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
        m_JoystickRectTransfom = GetComponent<RectTransform>();
        Debug.Log($"{m_JoystickRectTransfom.anchoredPosition}");

        m_Center = m_Background.anchoredPosition;
        m_Radius = m_Background.rect.width * 0.5f;
        m_BackgroundQuaternion = m_Background.rotation;
    }

    private void Update()
    {
        // �߽������κ��� ��ŭ �������ִ��� ����ȭ
        float x = 0f;
        float y = 0f;
        if (m_Lever.anchoredPosition.x != 0)
            x = (m_Lever.anchoredPosition.x - m_Radius) / m_Radius;
        if (m_Lever.anchoredPosition.y != 0)
            y = (m_Lever.anchoredPosition.y - m_Radius) / m_Radius;

        m_Input = new Vector2(x, y);
    }

    private void OnDisable()
    {
        ResetJoystick();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PointerId = eventData.pointerId;
        GameManager.InputSystem.ExternallyUsingFingerIds.Add(PointerId);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���� �̵�
        Vector2 diff = eventData.position - (m_Center + m_JoystickRectTransfom.anchoredPosition);

        // ������ ���̽�ƽ ������ ���̸�ŭ �̵���Ű�� (������ �̵��ϴ� �� ����)
        m_Lever.anchoredPosition = m_Center + (diff.normalized * m_Radius);

        // ���̽�ƽ ��׶��� �̵�
        Vector2 endPoint = new Vector2(m_Center.x, m_Center.y + m_Radius);
        Vector2 pivotAxis = (endPoint - m_Center).normalized;
        Vector2 leverPoint = new Vector2(m_Lever.anchoredPosition.x, m_Lever.anchoredPosition.y);
        Vector2 leverAxis = (leverPoint - m_Center).normalized;

        // ���Ϳ��� ������ ���ϴ� ���Ϳ� ���Ϳ��� ������ ���Ϳ��� ����
        float angle = Mathf.Abs(Vector3.Angle(pivotAxis, leverAxis));

        // ���������� ���̽�ƽ�� �ű���
        float deltaX = diff.x;
        if (deltaX > 0)
        {
            // ȸ���� -�� ������Ѵ�.
            m_Background.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_BackgroundQuaternion.eulerAngles.z - angle));
        }
        // �������� ���̽�ƽ�� �ű���
        else
        {
            // ȸ���� +�� ������Ѵ�.
            m_Background.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_BackgroundQuaternion.eulerAngles.z + angle));
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        ResetJoystick(eventData);
    }

    void ResetJoystick(PointerEventData eventData = null)
    {
        GameManager.InputSystem.ExternallyUsingFingerIds.Remove(PointerId);
        PointerId = -1;

        m_Lever.anchoredPosition = m_Center;
        m_Background.transform.rotation = m_BackgroundQuaternion;
    }
}
