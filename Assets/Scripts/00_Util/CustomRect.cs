using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomRect
{
    public float m_Width;
    public float m_Height;

    Vector2 m_Center;

    public CustomRect(Vector2 centerPosition)
    {
        m_Center = centerPosition;
        m_Width = 0;
        m_Height = 0;
    }

    public CustomRect(Vector2 centerPosition, int width, int height) : this(centerPosition)
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

    public bool IsPositionInRect(Vector2 position)
    {
        float minX = m_Center.x - (m_Width * 0.5f);
        float minY = m_Center.y - (m_Height * 0.5f);
        float maxX = m_Center.x + (m_Width * 0.5f);
        float maxY = m_Center.x + (m_Height * 0.5f);

        if (position.x >= minX && position.x <= maxX &&
            position.y >= minY && position.y <= maxY)
            return true;

        return false;
    }
}
