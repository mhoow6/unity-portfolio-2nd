using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �߽��� �簢�� ���߾��� Rect. ��ũ����ǥ�踦 ����Ѵ�.
/// </summary>
public struct CustomRect
{
    public float Width;
    public float Height;
    public Vector2 Center;

    public CustomRect(Vector2 centerPosition, float width, float height)
    {
        Center = centerPosition;
        Width = width;
        Height = height;
    }

    /// <summary> ��ũ����ǥ���� screenPos�� rect�ȿ� �ִ��� �˻��մϴ�. </summary>
    public bool IsScreenPointInRect(Vector2 screenPos)
    {
        float minX = Center.x - (Width * 0.5f);
        float minY = Center.y - (Height * 0.5f);
        float maxX = Center.x + (Width * 0.5f);
        float maxY = Center.y + (Height * 0.5f);

        if (screenPos.x >= minX && screenPos.x <= maxX &&
            screenPos.y >= minY && screenPos.y <= maxY)
            return true;

        return false;
    }
}
