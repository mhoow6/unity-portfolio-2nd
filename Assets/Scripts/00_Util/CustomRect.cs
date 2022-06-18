using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 중심이 사각형 정중앙인 Rect. 스크린좌표계를 사용한다.
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

    /// <summary> 스크린좌표계의 screenPos가 rect안에 있는지 검사합니다. </summary>
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
