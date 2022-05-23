using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomRect
{
    public float m_Width;
    public float m_Height;

    Vector2 m_Center;

    public CustomRect(Vector2 screenCenterPosition)
    {
        m_Center = screenCenterPosition;
        m_Width = 0;
        m_Height = 0;
    }

    public CustomRect(Vector2 screenCenterPosition, int width, int height) : this(screenCenterPosition)
    {
        m_Width = width;
        m_Height = height;
    }

    public CustomRect(Rect rect)
    {
        m_Center = new Vector2(rect.center.x - (rect.width * 0.5f), rect.center.y - (rect.height * 0.5f));
        m_Width = rect.width;
        m_Height = rect.height;
    }

    public bool IsMousePositionInRect(Vector2 mousePosition)
    {
        float minX = m_Center.x - (m_Width * 0.5f);
        float minY = m_Center.y - (m_Height * 0.5f);
        float maxX = m_Center.x + (m_Width * 0.5f);
        float maxY = m_Center.x + (m_Height * 0.5f);

        if (mousePosition.x >= minX && mousePosition.x <= maxX &&
            mousePosition.y >= minY && mousePosition.y <= maxY)
            return true;

        return false;
    }
}
